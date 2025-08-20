namespace MKW.Core.Storage
{
    public record class DatabaseSecretEntry
    {
        // User -> Payload
        public required IDictionary<Guid, Memory<byte>> Keys { get; set; }

        // Salt used within [decoded]key to encode Data
        public required Memory<byte> Salt { get; set; }

        // The payload, symmetrically encoded using a key, available by encoding
        // one of Keys using user's private key. The Salt is required to operate
        // (internally states as IV).
        public required Memory<byte> Data { get; set; }
    }
}
