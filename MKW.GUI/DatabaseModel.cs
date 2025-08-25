using MKW.Core.Client;
using MKW.Core.Client.Notify;
using MKW.Core.Storage;
using MKW.Core.Storage.JSON;

namespace MKW.GUI
{
    public class DatabaseModel : IDisposable
    {
        public string Path { get; }

        public IDatabase Database { get; }
        public ClientSession Client { get; }

        public UserSession? User { get; private set; }
        public AdminSession? Admin { get; private set; }

        public event EventHandler? OnEntriesChanged;

        public DatabaseModel(IDatabase database, string path, ClientSession client)
        {
            Path = path;
            Database = database;
            Client = client;
        }

        public static DatabaseModel Open(string path)
        {
            JSONDatabaseSession db = JSONDatabaseSession.Open(path, false);
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
        }

        public void CreateEntry(string payload)
        {
            using Entry entry = Client.CreateEntry();
            entry.UpdatePayload(new EntryPayload(payload));
            OnEntriesChanged?.Invoke(this, new EventArgs());
        }

        public void UpdateEntry(Entry entry, string text)
        {
            entry.UpdatePayload(new EntryPayload(text));
            OnEntriesChanged?.Invoke(this, new EventArgs());
        }

        public IEnumerable<DatabaseUserModel> EnumerateUsers()
        {
            yield return new DatabaseUserModel
            {
                Id = UserId.Admin(),
                IsAdmin = true,
                Name = "Admin"
            };

            foreach (UserInfo user in Client.EnumerateUsers())
            {
                yield return new DatabaseUserModel
                {
                    Id = user.Id,
                    IsAdmin = false,
                    Name = "User"
                };
            }
        }

        public void Dispose()
        {
            Database?.Dispose();
            Client?.Dispose();
            User?.Dispose();
        }
    }
}
