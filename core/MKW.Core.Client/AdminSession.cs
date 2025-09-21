using MKW.Core.Notify;
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
        , ITrustController
        , IDisposable
    {
        private readonly UserMetadataEncoder metadataEncoder;

        public AdminSession(ClientSession client /* reference */,
                            ICryptographyProvider crypto,
                            IDatabase database /* reference */,
                            IDatabaseUser admin /* reference */,
                            ReadOnlySpan<byte> privateKey)
            : base(client, crypto, database, admin, privateKey)
        {
            metadataEncoder = new UserMetadataEncoder();
        }

        public UserInfo CreateUser(UserAccessRequest request, UserMetadata metadata)
        {
            IDatabaseUser user = database.CreateUser(UserId.Create());

            user.Salt = request.Salt;
            user.PublicKey = request.PublicKey;
            user.PrivateKey = request.EncryptedPrivateKey;
            metadataEncoder.UpdateMetadata(user, metadata);
            user.AddTrust(request.AdminSignature);

            user.Save();

            // TODO: sign user
            // TODO: account admin signature

            return UserInfo.FromDatabaseUser(user);
        }
    }
}
