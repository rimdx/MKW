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
            DatabasePath = Path.GetFullPath("./test.json");

            // todo: safer way?
            File.Delete("./test.json");

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

        public AdminSession OpenAdmin(ClientSession client)
        {
            return client.OpenAdmin(AdminSecret);
        }

        public UserSession CreateUser(ClientSession client,
                                      string password,
                                      out UserInfo user,
                                      bool trusted = true)
        {
            using AdminSession admin = OpenAdmin(client);

            user = client.PromoteUser(password);

            UserSession userSession = client.OpenUser(user.Id, password);

            if (trusted)
            {
                admin.UpdateTrust(user.Id, Trust.ExplicitTrust);
                userSession.UpdateTrust(admin.Id, Trust.ExplicitTrust);
            }

            return userSession;
        }
    }
}
