using MKW.Core.Exceptions;
using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Implementation
{
    public class UserMetadataDecoder : IDisposable
    {
        private readonly IAsymmetricPublicTransformer adminKey;

        public UserMetadataDecoder(IAsymmetricPublicTransformer adminKey)
        {
            this.adminKey = adminKey;
        }

        public UserMetadata OpenMetadata(IDatabaseUser user)
        {
            if (adminKey.Verify(user.Metadata.Payload.Span, user.Metadata.Signature.Span))
            {
                return UserMetadataSerializer.Deserialize(user.Metadata.Payload.Span);
            }
            else
            {
                throw new InvalidUserMetadataSignature();
            }
        }

        public void Dispose()
        {
        }
    }
}
