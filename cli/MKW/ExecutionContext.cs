using MKW.Core;
using MKW.Core.Client;
using MKW.Core.Storage;
using MKW.Core.Storage.JSON;

namespace MKW
{
    public class ExecutionContext : IDisposable
    {
        private IDatabase? database;
        private ClientSession? client;

        private IUserSession? user;
        private IAdminSession? admin;

        public ExecutionContext()
        {
        }

        public ClientSession Client => client ?? throw new Exception("Client is not initialized.");
        public IUserSession User => user ?? throw new Exception("User is not initialized.");
        public IAdminSession Admin => admin ?? throw new Exception("Admin is not initialized.");

        public void OpenDatabase(string path)
        {
            database = JSONDatabaseSession.Open(path);
            client = ClientSession.Open(database);
        }

        public void CreateDatabase(string path,
                                   string adminPassword,
                                   UserMetadata adminMetadata)
        {
            database = JSONDatabaseSession.Create(path);

            client = ClientSession.Create(database,
                                          adminPassword,
                                          adminMetadata);
        }

        public void OpenUser(string password)
        {
            user = Client.OpenUser(password);
        }

        public void OpenAdmin(string password)
        {
            admin = Client.OpenAdmin(password);
            user = admin;
        }

        public void Dispose()
        {
            database?.Dispose();
            client?.Dispose();
        }
    }
}
