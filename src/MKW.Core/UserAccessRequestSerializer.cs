using System.Text.Json;
using System.Text.Json.Serialization;

namespace MKW.Core
{
    public partial class UserAccessRequestSerializer : IAccessRequestSerializer
    {
        [JsonSourceGenerationOptions()]
        [JsonSerializable(typeof(UserAccessRequestData))]
        private partial class SerializerContext : JsonSerializerContext
        {
        }

        public UserAccessRequestSerializer()
        {
        }

        public ReadOnlyMemory<byte> Serialize(UserAccessRequest data)
        {
            UserAccessRequestData obj = new UserAccessRequestData
            {
                Salt = data.Salt,
                PublicKey = data.PublicKey,
                PrivateKey = data.EncryptedPrivateKey,
                AdminSignature = data.AdminSignature,
            };

            return JsonSerializer.SerializeToUtf8Bytes(obj, SerializerContext.Default.UserAccessRequestData);
        }

        public UserAccessRequest Deserialize(ReadOnlySpan<byte> data)
        {
            UserAccessRequestData? parsed = JsonSerializer.Deserialize(data, SerializerContext.Default.UserAccessRequestData);

            if (parsed == null)
            {
                throw new NullReferenceException();
            }

            return new UserAccessRequest
            {
                Salt = parsed.Salt,
                PublicKey = parsed.PublicKey,
                EncryptedPrivateKey = parsed.PrivateKey,
                AdminSignature = parsed.AdminSignature,
            };
        }
    }
}
