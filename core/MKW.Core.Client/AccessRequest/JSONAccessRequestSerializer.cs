using System.Text.Json;
using System.Text.Json.Serialization;

namespace MKW.Core.Client.AccessRequest
{
    public partial class JSONAccessRequestSerializer : IAccessRequestSerializer
    {
        [JsonSourceGenerationOptions()]
        [JsonSerializable(typeof(JSONAccessRequestData))]
        private partial class SerializerContext : JsonSerializerContext
        {
        }

        public JSONAccessRequestSerializer()
        {
        }

        public ReadOnlyMemory<byte> Serialize(UserAccessRequest data)
        {
            JSONAccessRequestData obj = new JSONAccessRequestData
            {
                Salt = data.Salt,
                PublicKey = data.PublicKey,
                PrivateKey = data.PrivateKey,
            };

            return JsonSerializer.SerializeToUtf8Bytes(obj, SerializerContext.Default.JSONAccessRequestData);
        }

        public UserAccessRequest Deserialize(ReadOnlySpan<byte> data)
        {
            JSONAccessRequestData? parsed = JsonSerializer.Deserialize(data, SerializerContext.Default.JSONAccessRequestData);

            if (parsed == null)
            {
                throw new NullReferenceException();
            }

            return new UserAccessRequest
            {
                Salt = parsed.Salt,
                PublicKey = parsed.PublicKey,
                PrivateKey = parsed.PrivateKey,
            };
        }
    }
}
