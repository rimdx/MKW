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

        public AdminSession(ICryptographyProvider crypto,
                            IDatabase database /* reference */,
                            IDatabaseUser admin /* reference */,
                            ReadOnlySpan<byte> privateKey)
            : base(crypto, database, admin, privateKey)
        {
            metadataEncoder = new UserMetadataEncoder(Transformer);
            accessController = new UserAccessController(this);
        }

        public UserInfo CreateUser(UserAccessRequest request, UserMetadata metadata)
        {
            UserId userId = UserId.Create();

            IDatabaseUser user = database.CreateUser(userId);

            user.Salt = request.Salt;
            user.PublicKey = request.PublicKey;
            user.AdminTrustSignature = Transformer.Sign(request.PublicKey.Span);
            user.PrivateKey = request.EncryptedPrivateKey;
            user.Metadata = metadataEncoder.EncodeMetadata(metadata);

            user.AdminSignature = request.AdminSignature;

            user.Save();

            trustController.AddTrust(user);
            accessController.AddAccess(userId);

            return UserInfo.FromDatabaseUser(user);
        }
    }
}
