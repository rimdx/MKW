using MKW.Core.Client;
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

        public DatabaseModel(IDatabase database, string path)
        {
            Path = path;
            Database = database;
            Client = ClientSession.Open(database);
        }

        public static DatabaseModel Open(string path)
        {
            JSONDatabaseSession db = JSONDatabaseSession.Open(path, DatabaseOpenMode.Open);
            return new DatabaseModel(db, path);
        }

        public static DatabaseModel Create(string path)
        {
            JSONDatabaseSession db = JSONDatabaseSession.Open(path, DatabaseOpenMode.OpenOrCreate);
            return new DatabaseModel(db, path);
        }

        public void Authenticate(UserId id, string password)
        {
            User = Client.OpenUser(id, password);
        }

        public void CreateAdmin(string password)
        {
            Client.PromoteAdmin(password);
            Admin = Client.OpenAdmin(password);
            User = Admin;
        }

        public void Dispose()
        {
            Database?.Dispose();
            Client?.Dispose();
            User?.Dispose();
        }
    }
}
