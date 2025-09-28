using MKW.Core;
using MKW.Core.Client;
using MKW.Core.Storage;
using MKW.Core.Storage.JSON;
using MKW.Cryptography;

namespace MKW.GUI.Model
{
    public class DatabaseModel : ViewModelBase, IDisposable
    {
        public DatabaseModel(IDatabase database,
                             string path,
                             ClientSession client)
        {
            Database = database;
            Path = path;
            Client = client;

            users = [.. EnumerateUsers()];

            pullDatabaseUpdatesCancellationSource = new CancellationTokenSource();
            _ = PullDatabaseUpdates(pullDatabaseUpdatesCancellationSource.Token);
        }

        private readonly CancellationTokenSource pullDatabaseUpdatesCancellationSource;

        public IDatabase Database { get; }
        public string Path { get; }
        public ClientSession Client { get; }

        private bool wantRefresh = false;
        public bool WantRefresh
        {
            get => wantRefresh;
            private set => SetProperty(ref wantRefresh, value);
        }

        private DatabaseUnlockedModel? unlockedModel;
        public DatabaseUnlockedModel? UnlockedDatabase
        {
            get => unlockedModel;
            private set
            {
                DatabaseUnlockedModel? oldValue = unlockedModel;

                if (SetProperty(ref unlockedModel, value))
                {
                    oldValue?.Dispose();
                }
            }
        }

        private IReadOnlyCollection<DatabaseUserModel> users;
        public IReadOnlyCollection<DatabaseUserModel> Users
        {
            get => users;
            private set => SetProperty(ref users, value);
        }

        public static DatabaseModel Open(ICryptographyProvider crypto, string path)
        {
            JSONDatabaseSession database = JSONDatabaseSession.Open(path);
            ClientSession client = ClientSession.Open(database, crypto);
            return new DatabaseModel(database, path, client);
        }

        public static DatabaseModel Create(ICryptographyProvider crypto, string path, string adminPassword)
        {
            UserMetadata metadata = new UserMetadata // todo
            {
                DisplayName = "",
                UserId = ""
            };

            JSONDatabaseSession database = JSONDatabaseSession.Create(path);
            ClientSession client = ClientSession.Create(database, crypto, adminPassword, metadata);
            DatabaseModel model = new DatabaseModel(database, path, client);

            return model;
        }

        private async Task PullDatabaseUpdates(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (await Database.WaitForDatabaseChangesAsync(cancellationToken))
                    {
                        WantRefresh = true;
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // no-op
            }
        }

        public void ReloadDatabaseFile()
        {
            Database.ReloadDatabaseFile();

            WantRefresh = false;

            RefreshUsers();
            UnlockedDatabase?.RefreshEntries();
        }

        public DatabaseUnlockedModel Unlock(UserId id, string password)
        {
            IUserSession user = Client.OpenUser(id, password);
            UnlockedDatabase = new DatabaseUnlockedModel(this, user);
            return UnlockedDatabase;
        }

        public void Lock()
        {
            UnlockedDatabase = null;
        }

        private IEnumerable<DatabaseUserModel> EnumerateUsers()
        {
            foreach (UserInfo user in Client.EnumerateUsers())
            {
                yield return new DatabaseUserModel(user);
            }
        }

        internal void RefreshUsers()
        {
            Users = [.. EnumerateUsers()];
        }

        public UserAccessRequest CreateUserAccessRequest(string password)
        {
            return Client.CreateUserAccessRequest(password);
        }

        public void Dispose()
        {
            pullDatabaseUpdatesCancellationSource.Cancel();
        }
    }
}
