using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    public class UserMetadataEncoder
    {
        private readonly IAsymmetricPrivateTransformer adminKey;

        public UserMetadataEncoder(IAsymmetricPrivateTransformer adminKey)
        {
            this.adminKey = adminKey;
        }

        public void UpdateMetadata(IDatabaseUser user, UserMetadata metadata)
        {
            ReadOnlyMemory<byte> encoded = UserMetadataSerializer.Serialize(metadata);
            ReadOnlyMemory<byte> signature = adminKey.Sign(encoded.Span);

            user.Metadata = encoded;
            user.MetadataAdminSignature = signature;
        }
    }
}
