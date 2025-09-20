using System.Text.Json;

namespace MKW.Core
{
    public static class UserMetadataSerializer
    {
        public static UserMetadata Deserialize(ReadOnlySpan<byte> data)
        {
            UserMetadata? metadata = JsonSerializer.Deserialize<UserMetadata>(data);

            if (metadata == null)
            {
                throw new NullReferenceException();
            }

            return metadata;
        }

        public static ReadOnlyMemory<byte> Serialize(UserMetadata metadata)
        {
            return JsonSerializer.SerializeToUtf8Bytes(metadata);
        }
    }
}
