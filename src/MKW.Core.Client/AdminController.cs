using MKW.Core.Implementation;
using MKW.Core.Storage;
using MKW.Cryptography;
using MKW.Cryptography.Exceptions;

namespace MKW.Core.Client
{
    public class AdminController : IAdminController, IDisposable
    {
        private readonly ICryptographyProvider crypto;
        private readonly IDatabase database;

        public AdminController(ICryptographyProvider crypto,
                               IDatabase database)
        {
            this.crypto = crypto;
            this.database = database;
        }

        public UserInfo CreateAdmin(string password, UserMetadata metadata)
        {
            SystemCredentialsManager credManager = new SystemCredentialsManager(crypto);

            IUserCredentials userCreds = crypto.CreateUserCredentials(password);
            using SystemCredentials systemCreds = credManager.GenerateCredentials(userCreds);

            IDatabaseUser admin = database.CreateUser(UserId.Admin());

            UserMetadataEncoder metadataEncoder = new UserMetadataEncoder(systemCreds.Transformer);

            admin.PublicKey = systemCreds.PublicKey;
            admin.PrivateKey = systemCreds.PrivateKey;
            admin.Salt = systemCreds.Salt;
            admin.Metadata = metadataEncoder.EncodeMetadata(metadata);

            admin.Save();

            return UserInfo.FromDatabaseUser(admin);
        }

        public IAdminSession OpenAdmin(string password)
        {
            try
            {
                IDatabaseUser admin = database.OpenUser(UserId.Admin(), false);

                IUserCredentials creds = crypto.OpenUserCredentials(password, admin.Salt);

                using ISymmetricTransformer decoder = crypto.OpenSymmetricTransformer(
                    creds.GetSecretKey().Span, creds.ExportSalt().Span);

                Memory<byte> privateKeyBytes = decoder.Decrypt(admin.PrivateKey.Span);

                return new AdminSession(crypto, database, admin, privateKeyBytes.Span);
            }
            catch (SymmetricOperationFailedException ex)
            {
                throw new Exceptions.InvalidPasswordException(ex);
            }
            catch (InvalidKeyException ex)
            {
                // Possible occurrence, as experiments have shown. Fails in 1/~35 times.
                throw new Exceptions.InvalidPasswordException(ex);
            }
        }

        public UserInfo GetAdminInfo()
        {
            IDatabaseUser admin = database.OpenUser(UserId.Admin(), true);
            return UserInfo.FromDatabaseUser(admin);
        }

        public void Dispose()
        {
        }
    }
}
