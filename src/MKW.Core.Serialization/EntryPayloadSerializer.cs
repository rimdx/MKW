using System.Text;

namespace MKW.Core.Serialization
{
    public static class EntryPayloadSerializer
    {
        public static EntryPayload Deserialize(ReadOnlySpan<byte> data)
        {
            return new EntryPayload
            {
                Notes = Encoding.UTF8.GetString(data.ToArray()),
            };
        }

        public static ReadOnlyMemory<byte> Serialize(EntryPayload payload)
        {
            return Encoding.UTF8.GetBytes(payload.Notes);
        }
    }
}
