using MKW.Core.Implementation;
using MKW.Storage;
using MKW.Cryptography;

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
            SystemCredentialsManager credManager = new SystemCredentialsManager(crypto);

            DatabaseUser admin = database.OpenUser(UserId.Admin());

            IUserCredentials creds = crypto.OpenUserCredentials(password,
                                                                admin.Salt,
                                                                CommonCryptographyAlgorithms.Pbkdf2);

            SystemCredentials systemCreds = credManager.OpenCredentials(admin, creds);

            return new AdminSession(crypto, database, admin, systemCreds.Transformer);
        }

        public UserInfo GetAdminInfo()
        {
            DatabaseUser admin = database.OpenUser(UserId.Admin());

            using IAsymmetricPublicTransformer adminKey = crypto.OpenAsymmetricTransformer(
                admin.PublicKey.Payload.Span,
                CommonCryptographyAlgorithms.Rsa2048);

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
