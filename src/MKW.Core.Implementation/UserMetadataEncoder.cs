using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Implementation
{
    public class UserMetadataEncoder
    {
        private readonly IAsymmetricPrivateTransformer adminKey;

        public UserMetadataEncoder(IAsymmetricPrivateTransformer adminKey)
        {
            this.adminKey = adminKey;
        }

        public SignedPayload EncodeMetadata(UserMetadata metadata)
        {
            ReadOnlyMemory<byte> encoded = UserMetadataSerializer.Serialize(metadata);
            ReadOnlyMemory<byte> signature = adminKey.Sign(encoded.Span);

            return new SignedPayload(encoded, signature);
        }
    }
}
