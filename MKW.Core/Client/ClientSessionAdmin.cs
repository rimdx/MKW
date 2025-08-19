using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;
using System.Security.Cryptography;

namespace MKW.Core.Client
{
    public partial class ClientSession : IDisposable
    {
        public UserInfo PromoteAdmin(string password)
        {
            SystemCredentialsManager credManager = new SystemCredentialsManager();

            UserCredentials userCreds = UserCredentials.Create(password);
            SystemCredentials systemCreds = credManager.GenerateCredentials(userCreds);

            AdminUser user = new AdminUser
            {
                Id = Guid.NewGuid(),
                PublicKey = systemCreds.PublicKey,
                PrivateKey = systemCreds.PrivateKey,
                Salt = systemCreds.Salt,
            };

            db.UpdateAdmin(user);

            return UserInfo.FromDatabaseUser(user);
        }

        public AdminSession OpenAdmin(string password)
        {
            AdminUser admin = db.GetAdmin();

            UserCredentials creds = UserCredentials.Open(password, admin.Salt);

            using SymmetricTransformer decoder = SymmetricTransformer.Open(creds.GetEncodingHash(),
                                                                           creds.ExportSalt());

            byte[] privateKeyBytes = decoder.Decrypt(admin.PrivateKey);

            return new AdminSession(db, admin, privateKeyBytes);
        }
    }
}
