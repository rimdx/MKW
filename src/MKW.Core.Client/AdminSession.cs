using MKW.Core.Implementation;
using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    public class AdminSession
        : UserSession
        , IAdminSession
        , IUserSession
        , IUserHost
        , IEntryController
        , ITrustProvider
        , IDisposable
    {
        private readonly UserMetadataEncoder metadataEncoder;
        private readonly UserAccessController accessController;
        private readonly UserTrustController trustController;

        public AdminSession(ICryptographyProvider crypto,
                            IDatabase database /* reference */,
                            IDatabaseUser admin /* reference */,
                            ReadOnlySpan<byte> privateKey)
            : base(crypto, database, admin, privateKey)
        {
            metadataEncoder = new UserMetadataEncoder(Transformer);
            accessController = new UserAccessController(this);
            trustController = new UserTrustController(database, crypto, Transformer);
        }

        public UserInfo CreateUser(UserAccessRequest request, UserMetadata metadata)
        {
            UserId userId = UserId.Create();

            IDatabaseUser user = database.CreateUser(userId);

            user.Salt = request.Salt;
            user.PublicKey = request.PublicKey;
            user.PrivateKey = request.EncryptedPrivateKey;
            metadataEncoder.UpdateMetadata(user, metadata);

            user.AddTrust(request.AdminSignature);
            user.Save();

            trustController.AddTrust(user);
            accessController.AddAccess(userId);

            return UserInfo.FromDatabaseUser(user);
        }
    }
}
