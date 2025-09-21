using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    public class UserMetadataEncoder
    {
        public UserMetadataEncoder()
        {
        }

        public void UpdateMetadata(IDatabaseUser user, UserMetadata metadata)
        {
            ReadOnlyMemory<byte> encoded = UserMetadataSerializer.Serialize(metadata);

            user.Metadata = encoded;
        }
    }
}
