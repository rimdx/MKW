namespace MKW.Core.Storage
{
    public record class Entry
    {
        // User -> Payload
        public required IDictionary<Guid, byte[]> Keys { get; set; }

        public required byte[] Data { get; set; }
    }
}
