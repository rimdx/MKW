using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class UserMetadataDecoder : IDisposable
    {
        private readonly IDatabaseUser user;

        public UserMetadataDecoder(IDatabaseUser user)
        {
            this.user = user;
        }

        public UserMetadata OpenMetadata()
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
