using MKW.Core;
using MKW.Core.Client;
using MKW.Core.Storage;
using MKW.Core.Storage.JSON;

namespace MKW.GUI.Model
{
    public class DatabaseUnlockedModel : ViewModelBase, IDisposable
    {
        private IDatabase database;
        private IUserSession? user;
        private IAdminSession? admin;

        public string Path { get; }

        public ClientSession Client { get; }

        public IUserSession? User 
        {
            get => user;
            private set => SetProperty(ref user, value);
        }

        public IAdminSession? Admin
        {
            get => admin;
            private set => SetProperty(ref admin, value);
        }

        public event EventHandler? OnEntriesChanged;
        public event EventHandler? OnUsersChanged;

        private DatabaseUnlockedModel(IDatabase database, string path, ClientSession client)
        {
            this.database = database;
            Path = path;
            Client = client;
        }

        public static DatabaseUnlockedModel Open(string path)
        {
            JSONDatabaseSession db = JSONDatabaseSession.Open(path);
            ClientSession client = ClientSession.Open(db);
            return new DatabaseUnlockedModel(db, path, client);
        }

        public static DatabaseUnlockedModel Create(string path, string adminPassword)
        {
            UserMetadata metadata = new UserMetadata // todo
            {
                DisplayName = "",
                UserId = ""
            };

            JSONDatabaseSession db = JSONDatabaseSession.Create(path);
            ClientSession client = ClientSession.Create(db, adminPassword, metadata);

            DatabaseUnlockedModel model = new DatabaseUnlockedModel(db, path, client);

            model.Admin = model.Client.OpenAdmin(adminPassword);
            model.User = model.Admin;

            return model;
        }

        public void Unlock(UserId id, string password)
        {
            User = Client.OpenUser(id, password);

            if (User is IAdminSession admin)
            {
                Admin = admin;
            }
        }

        public void Lock()
        {
            User = null;
            Admin = null;
        }

        public void CreateEntry(string payload)
        {
            using IEntrySession entry = User!.CreateEntry();
            entry.UpdatePayload(new EntryPayload(payload));
            OnEntriesChanged?.Invoke(this, new EventArgs());
        }

        public void UpdateEntry(IEntrySession entry, string text)
        {
            entry.UpdatePayload(new EntryPayload(text));
            OnEntriesChanged?.Invoke(this, new EventArgs());
        }

        public void DeleteEntry(EntryId id)
        {
            database.DeleteEntry(id);
            OnEntriesChanged?.Invoke(this, new EventArgs());
        }

        private Trust GetTrust(UserInfo user)
        {
            if (User == null)
            {
                return Trust.Unknown;
            }
            else
            {
                // TODO:
                return User.VerifyTrust(user.Id) ? Trust.ExplicitTrust : Trust.None;
            }
        }

        public IEnumerable<DatabaseUserModel> EnumerateUsers()
        {
            foreach (UserInfo user in Client.EnumerateUsers())
            {
                yield return new DatabaseUserModel(user, GetTrust(user));
            }
        }

        public void AddUser(UserAccessRequest request, UserMetadata userMetadata)
        {
            if (Admin == null)
            {
                throw new Exception("Not an admin.");
            }

            UserInfo user = Admin.CreateUser(request, userMetadata);

            OnUsersChanged?.Invoke(this, new EventArgs());
        }

        public UserEditorModel CreateUserEditor(UserId userId)
        {
            UserInfo user = Client.GetUserInfo(userId);
            return new UserEditorModel(this, user);
        }

        public void DeleteUser(UserId id)
        {
            database.DeleteUser(id);
            OnUsersChanged?.Invoke(this, new EventArgs());
        }

        public void Dispose()
        {
            User?.Dispose();
            Client?.Dispose();
            database?.Dispose();
        }
    }
}
