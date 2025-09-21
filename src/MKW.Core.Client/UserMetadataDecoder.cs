using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class UserMetadataDecoder : IDisposable
    {
        public UserMetadataDecoder()
        {
        }

        public UserMetadata OpenMetadata(IDatabaseUser user)
        {
            UserMetadata metadata = UserMetadataSerializer.Deserialize(user.Metadata.Span);

            // TODO: verify

            return metadata;
        }

        public void Dispose()
        {
        }
    }
}
