using MKW.Core;
using MKW.Core.Client;
using MKW.Core.Notify;
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
                UserMetadata metadata = new UserMetadata
                {
                    DisplayName = "admin",
                    UserId = "admin@contoso.com"
                };

                using JSONDatabaseSession db = JSONDatabaseSession.Create(DatabasePath);
                using ClientSession client = ClientSession.Create(db, AdminSecret, metadata);
            }
        }

        public IDatabase OpenDatabase()
        {
            return JSONDatabaseSession.Open(DatabasePath);
        }

        public ClientSession OpenSession()
        {
            return ClientSession.Open(OpenDatabase(), true);
        }

        public IAdminSession OpenAdmin(ClientSession client)
        {
            return client.OpenAdmin(AdminSecret);
        }

        public IUserSession CreateUser(ClientSession client,
                                       string password,
                                       out UserInfo user,
                                       bool trusted = true)
        {
            using IAdminSession admin = OpenAdmin(client);

            UserAccessRequest request = client.CreateUserAccessRequest(password);

            user = admin.CreateUser(
                request,
                new UserMetadata
                {
                    UserId = $"{password}@privatetestgang.com",
                    DisplayName = password
                });

            IUserSession userSession = client.OpenUser(user.Id, password);

            if (trusted)
            {
                admin.AddTrust(user.Id);
            }
            else
            {
            }

            return userSession;
        }
    }
}
