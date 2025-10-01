using System.Text.Json;

namespace MKW.Core.Serialization
{
    public static class UserAccessRequestSerializer
    {
        public static ReadOnlyMemory<byte> Serialize(UserAccessRequest data)
        {
            UserAccessRequestData obj = new UserAccessRequestData
            {
                Salt = data.Salt,
                PublicKey = data.PublicKey,
                PrivateKey = data.EncryptedPrivateKey.EncryptedPayload,
                AdminSignature = data.AdminSignature,
            };

            return JsonSerializer.SerializeToUtf8Bytes(
                obj, UserAccessRequestJsonSerializerContext.Default.UserAccessRequestData);
        }

        public static UserAccessRequest Deserialize(ReadOnlySpan<byte> data)
        {
            UserAccessRequestData? parsed = JsonSerializer.Deserialize(
                data, UserAccessRequestJsonSerializerContext.Default.UserAccessRequestData);

            if (parsed == null)
            {
                throw new NullReferenceException();
            }

            return new UserAccessRequest
            {
                Salt = parsed.Salt,
                PublicKey = parsed.PublicKey,
                EncryptedPrivateKey = new SecretPayload(parsed.PrivateKey),
                AdminSignature = parsed.AdminSignature,
            };
        }
    }
}
