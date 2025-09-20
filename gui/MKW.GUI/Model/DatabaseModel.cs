using MKW.Core;
using MKW.Core.Client;
using MKW.Core.Notify;
using MKW.Core.Storage;
using MKW.Core.Storage.JSON;

namespace MKW.GUI.Model
{
    public class DatabaseModel : IDisposable
    {
        public string Path { get; }

        public IDatabase Database { get; }
        public ClientSession Client { get; }

        public IUserSession? User { get; private set; }
        public IUserSession? Admin { get; private set; }

        public event EventHandler? OnEntriesChanged;
        public event EventHandler? OnUsersChanged;

        private DatabaseModel(IDatabase database, string path, ClientSession client)
        {
            Path = path;
            Database = database;
            Client = client;
        }

        public static DatabaseModel Open(string path)
        {
            JSONDatabaseSession db = JSONDatabaseSession.Open(path);
            ClientSession client = ClientSession.Open(db);
            return new DatabaseModel(db, path, client);
        }

        public static DatabaseModel Create(string path, string adminPassword)
        {
            JSONDatabaseSession db = JSONDatabaseSession.Create(path);
            ClientSession client = ClientSession.Create(db, adminPassword);

            DatabaseModel model = new DatabaseModel(db, path, client);

            model.Admin = model.Client.OpenAdmin(adminPassword);
            model.User = model.Admin;

            return model;
        }

        public void Authenticate(UserId id, string password)
        {
            User = Client.OpenUser(id, password);

            if (User.Id.IsAdmin)
            {
                Admin = User;
            }
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
            Database.DeleteEntry(id);
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
                return User.GetImplicitTrust(user.Id);
            }
        }

        public IEnumerable<DatabaseUserModel> EnumerateUsers()
        {
            foreach (UserInfo user in Client.EnumerateUsers())
            {
                yield return new DatabaseUserModel(user, GetTrust(user));
            }
        }

        public void AddUser(UserAccessRequest request)
        {
            if (Admin == null)
            {
                throw new Exception("Not an admin.");
            }

            UserInfo user = Client.CreateUser(request, null);
            // TODO: rollback if failed
            Admin.AddTrust(user.Id);

            OnUsersChanged?.Invoke(this, new EventArgs());
        }

        public UserEditorModel CreateUserEditor(UserId userId)
        {
            UserInfo user = Client.GetUserInfo(userId);
            return new UserEditorModel(this, user);
        }

        public void DeleteUser(UserId id)
        {
            Database.DeleteUser(id);
            OnUsersChanged?.Invoke(this, new EventArgs());
        }

        public void Dispose()
        {
            Database?.Dispose();
            Client?.Dispose();
            User?.Dispose();
        }
    }
}
