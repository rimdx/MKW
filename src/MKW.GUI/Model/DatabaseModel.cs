using MKW.Core;
using MKW.Core.Client;
using MKW.Core.Storage;
using MKW.Core.Storage.JSON;
using MKW.GUI.Images;

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
        }

        public IDatabase Database { get; }
        public string Path { get; }
        public ClientSession Client { get; }

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

        public static DatabaseModel Open(string path)
        {
            JSONDatabaseSession database = JSONDatabaseSession.Open(path);
            ClientSession client = ClientSession.Open(database);
            return new DatabaseModel(database, path, client);
        }

        public static DatabaseModel Create(string path, string adminPassword)
        {
            UserMetadata metadata = new UserMetadata // todo
            {
                DisplayName = "",
                UserId = ""
            };

            JSONDatabaseSession database = JSONDatabaseSession.Create(path);
            ClientSession client = ClientSession.Create(database, adminPassword, metadata);

            return new DatabaseModel(database, path, client);
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

        private Trust GetTrust(UserInfo user)
        {
            if (UnlockedDatabase == null)
            {
                return Trust.Unknown;
            }
            else
            {
                return UnlockedDatabase.GetTrust(user);
            }
        }

        private IEnumerable<DatabaseUserModel> EnumerateUsers()
        {
            foreach (UserInfo user in Client.EnumerateUsers())
            {
                yield return new DatabaseUserModel(user, GetTrust(user));
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
        }
    }
}
