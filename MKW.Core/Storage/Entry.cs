namespace MKW.Core.Storage
{
    public record class Entry
    {
        // User -> Payload
        public required IDictionary<Guid, byte[]> Keys { get; set; }

        // The payload, symmetrically encoded using a key, available by encoding
        // one of Keys using user's private key. The Salt is required to operate
        // (internally states as IV).
        public required byte[] Data { get; set; }
    }
}
