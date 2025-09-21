using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    public class AdminSession
        : UserSession
        , IAdminSession
        , IUserSession
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
    }
}
