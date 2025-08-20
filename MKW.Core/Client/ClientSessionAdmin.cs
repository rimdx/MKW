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

            using IDatabaseAdmin admin = Database.OpenAdmin();

            admin.PublicKey = systemCreds.PublicKey;
            admin.PrivateKey = systemCreds.PrivateKey;
            admin.Salt = systemCreds.Salt;

            return UserInfo.FromDatabaseUser(admin);
        }

        public AdminSession OpenAdmin(string password)
        {
            IDatabaseAdmin admin = Database.OpenAdmin();

            UserCredentials creds = UserCredentials.Open(password, admin.Salt);

            using SymmetricTransformer decoder = SymmetricTransformer.Open(creds.GetEncodingHash().Span,
                                                                           creds.ExportSalt().Span);

            Memory<byte> privateKeyBytes = decoder.Decrypt(admin.PrivateKey.Span);

            return new AdminSession(Database, admin, privateKeyBytes.Span);
        }
    }
}
