using MKW.Core.Implementation;
using MKW.Core.Storage;
using MKW.Cryptography;
using MKW.Cryptography.Exceptions;

namespace MKW.Core.Client
{
    internal class AdminController : IAdminController, IDisposable
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

            UserMetadataEncoder metadataEncoder = new UserMetadataEncoder(systemCreds.Transformer);

            DatabaseUser admin = new DatabaseUser
            {
                Id = UserId.Admin(),
                PublicKey = new SignedPayload(systemCreds.PublicKey, null),
                PrivateKey = systemCreds.PrivateKey,
                Salt = systemCreds.Salt,
                Metadata = metadataEncoder.EncodeMetadata(metadata),
                AdminSignature = null, // TODO
            };

            database.CreateUser(UserId.Admin(), admin);

            return CreateUserInfo(admin, metadata);
        }

        public IAdminSession OpenAdmin(string password)
        {
            try
            {
                DatabaseUser admin = database.OpenUser(UserId.Admin());

                IUserCredentials creds = crypto.OpenUserCredentials(password, admin.Salt);

                using ISymmetricTransformer decoder = crypto.OpenSymmetricTransformer(
                    creds.GetSecretKey().Span, creds.ExportSalt().Span);

                Memory<byte> privateKeyBytes = decoder.Decrypt(admin.PrivateKey.EncryptedPayload.Span);

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
            DatabaseUser admin = database.OpenUser(UserId.Admin());

            using IAsymmetricPublicTransformer adminKey = crypto.OpenAsymmetricTransformer(
                admin.PublicKey.Payload.Span);

            UserMetadataDecoder metadataDecoder = new UserMetadataDecoder(adminKey);

            return CreateUserInfo(admin, metadataDecoder.OpenMetadata(admin));
        }

        public void Dispose()
        {
        }

        private static UserInfo CreateUserInfo(DatabaseUser user, UserMetadata metadata, Trust trust = Trust.Unknown)
        {
            return new UserInfo
            {
                Id = user.Id,
                PublicKey = user.PublicKey.Payload,
                Trust = trust,
                Metadata = metadata
            };
        }
    }
}
