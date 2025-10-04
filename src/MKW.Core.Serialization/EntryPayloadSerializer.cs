using System.Text;

namespace MKW.Core.Serialization
{
    public static class EntryPayloadSerializer
    {
        public static EntryPayload Deserialize(ReadOnlySpan<byte> data)
        {
            EntryPayload result = new EntryPayload();
            string content = Encoding.UTF8.GetString(data.ToArray());
            result.SetProperty(EntryPayloadCommonProperties.Notes, content);
            return result;
        }

        public static ReadOnlyMemory<byte> Serialize(EntryPayload payload)
        {
            return Encoding.UTF8.GetBytes(payload.GetProperty(EntryPayloadCommonProperties.Notes));
        }
    }
}
