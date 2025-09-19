using System.Text.Json;

namespace MKW.Core.Client.AccessRequest
{
    public class JSONAccessRequestSerializer : IAccessRequestSerializer
    {
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

            return JsonSerializer.SerializeToUtf8Bytes(obj);
        }

        public UserAccessRequest Deserialize(ReadOnlySpan<byte> data)
        {
            JSONAccessRequestData? parsed = JsonSerializer.Deserialize<JSONAccessRequestData>(data);

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
