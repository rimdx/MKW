using MKW.Core.Client;
using MKW.Core.Client.Notify;
using MKW.Core.Storage;
using MKW.Core.Storage.JSON;
using MKW.Cryptography;
using MKW.Cryptography.Loader;
using MKW.Testing.Common;

namespace MKW.Testing.Client
{
    public class ClientSandBox : SandBoxBase
    {
        public string AdminSecret => "adminsecret123";

        public ICryptographyProvider Crypto = CryptographyLoader.GetProvider();

        public ClientSandBox(bool init = true)
        {
            if (init)
            {
                using JSONDatabaseSession db = JSONDatabaseSession.Create(DatabasePath);
                using ClientSession client = ClientSession.Create(db, AdminSecret);
            }
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
