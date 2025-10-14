using MKW.Core.Implementation;
using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Client
{
    internal sealed class AdminController : IDisposable
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

            IUserCredentials userCreds = crypto.CreateUserCredentials(password,
                                                                      CommonCryptographyAlgorithms.Pbkdf2);

            SystemCredentials systemCreds = credManager.GenerateCredentials(userCreds);

            using IAsymmetricPrivateTransformer transformer = crypto.OpenAsymmetricTransformer(
                systemCreds.PrivateKey, CommonCryptographyAlgorithms.Rsa2048);

            UserMetadataEncoder metadataEncoder = new UserMetadataEncoder(transformer);

            DatabaseUser admin = new DatabaseUser
            {
                Id = UserId.Admin(),
                PublicKey = new SignedPayload(systemCreds.PublicKey, null),
                PrivateKey = systemCreds.EncryptedPrivateKey,
                Salt = systemCreds.Salt,
                Metadata = metadataEncoder.EncodeMetadata(metadata),
                AdminSignature = null, // TODO
            };

            database.CreateUser(UserId.Admin(), admin);

            return CreateUserInfo(admin, metadata);
        }

        public IAdminSession OpenAdmin(string password)
        {
            SystemCredentialsManager credManager = new SystemCredentialsManager(crypto);

            DatabaseUser admin = database.OpenUser(UserId.Admin());

            IUserCredentials creds = crypto.OpenUserCredentials(password,
                                                                admin.Salt,
                                                                CommonCryptographyAlgorithms.Pbkdf2);

            SystemCredentials systemCreds = credManager.OpenCredentials(admin, creds);

            IAsymmetricPrivateTransformer transformer = crypto.OpenAsymmetricTransformer(
                systemCreds.PrivateKey, CommonCryptographyAlgorithms.Rsa2048);

            return new AdminSession(crypto, database, admin, transformer);
        }

        public UserInfo GetAdminInfo()
        {
            DatabaseUser admin = database.OpenUser(UserId.Admin());

            AsymmetricPublicKey decodedKey = crypto.DecodePkcsPublicKey(admin.PublicKey.Payload.Span);

            using IAsymmetricPublicTransformer adminKey = crypto.OpenAsymmetricTransformer(
                decodedKey, CommonCryptographyAlgorithms.Rsa2048);

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
