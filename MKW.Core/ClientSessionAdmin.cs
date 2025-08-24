using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public partial class ClientSession : IDisposable
    {
        public UserInfo PromoteAdmin(string password)
        {
            SystemCredentialsManager credManager = new SystemCredentialsManager();

            UserCredentials userCreds = UserCredentials.Create(password);
            SystemCredentials systemCreds = credManager.GenerateCredentials(userCreds);

            IDatabaseUser admin = Database.CreateAdmin();

            admin.PublicKey = systemCreds.PublicKey;
            admin.PrivateKey = systemCreds.PrivateKey;
            admin.Salt = systemCreds.Salt;

            admin.Save();

            return UserInfo.FromDatabaseUser(admin);
        }

        public AdminSession OpenAdmin(string password)
        {
            IDatabaseUser admin = Database.OpenAdmin(false);

            UserCredentials creds = UserCredentials.Open(password, admin.Salt);

            using SymmetricTransformer decoder = SymmetricTransformer.Open(creds.GetSecretKey().Span,
                                                                           creds.ExportSalt().Span);

            Memory<byte> privateKeyBytes = decoder.Decrypt(admin.PrivateKey.Span);

            return new AdminSession(this, admin, privateKeyBytes.Span);
        }

        public UserInfo GetAdminInfo()
        {
            IDatabaseUser admin = Database.OpenAdmin(true);
            return UserInfo.FromDatabaseUser(admin);
        }
    }
}
