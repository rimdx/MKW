using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class AdminController : IAdminController, IDisposable
    {
        private readonly ClientSession client;
        private readonly IDatabase database;

        public AdminController(ClientSession client, IDatabase database)
        {
            this.client = client;
            this.database = database;
        }

        public UserInfo PromoteAdmin(string password)
        {
            SystemCredentialsManager credManager = new SystemCredentialsManager();

            UserCredentials userCreds = UserCredentials.Create(password);
            SystemCredentials systemCreds = credManager.GenerateCredentials(userCreds);

            IDatabaseUser admin = database.CreateAdmin();

            admin.PublicKey = systemCreds.PublicKey;
            admin.PrivateKey = systemCreds.PrivateKey;
            admin.Salt = systemCreds.Salt;

            admin.Save();

            return UserInfo.FromDatabaseUser(admin);
        }

        public AdminSession OpenAdmin(string password)
        {
            IDatabaseUser admin = database.OpenAdmin(false);

            UserCredentials creds = UserCredentials.Open(password, admin.Salt);

            using SymmetricTransformer decoder = SymmetricTransformer.Open(creds.GetSecretKey().Span,
                                                                           creds.ExportSalt().Span);

            Memory<byte> privateKeyBytes = decoder.Decrypt(admin.PrivateKey.Span);

            return new AdminSession(client, admin, privateKeyBytes.Span);
        }

        public UserInfo GetAdminInfo()
        {
            IDatabaseUser admin = database.OpenAdmin(true);
            return UserInfo.FromDatabaseUser(admin);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
