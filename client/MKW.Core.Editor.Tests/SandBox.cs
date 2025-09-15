using MKW.Core.Client;
using MKW.Core.Client.Notify;
using MKW.Core.Storage;
using MKW.Core.Storage.JSON;

namespace MKW.Core.Editor.Tests
{
    public class SandBox : IDisposable
    {
        public string DatabasePath { get; }
        public string AdminSecret => "adminsecret123";

        public SandBox(bool init = true)
        {
            DatabasePath = Path.GetTempFileName();

            if (init)
            {
                using JSONDatabaseSession db = JSONDatabaseSession.Create(DatabasePath);
                using ClientSession client = ClientSession.Create(db, AdminSecret);
            }
        }

        public void Dispose()
        {
        }

        public IDatabase OpenDatabase()
        {
            return JSONDatabaseSession.Open(DatabasePath, false);
        }

        public ClientSession OpenSession()
        {
            return ClientSession.Open(OpenDatabase(), true);
        }

        public IUserSession OpenAdmin(ClientSession client)
        {
            return client.OpenAdmin(AdminSecret);
        }

        public IUserSession CreateUser(ClientSession client,
                                       string password,
                                       out UserInfo user,
                                       bool trusted = true)
        {
            using IUserSession admin = OpenAdmin(client);

            user = client.PromoteUser(password);

            IUserSession userSession = client.OpenUser(user.Id, password);

            if (trusted)
            {
                admin.AddTrust(user.Id);
                userSession.AddTrust(admin.Id);
            }

            return userSession;
        }
    }
}
