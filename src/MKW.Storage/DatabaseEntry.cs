using MKW.Core;

namespace MKW.Storage
{
    public record class DatabaseEntry
    {
        public required EntryId Id { get; init; }

        // User -> Payload
        public required IDictionary<UserId, ReadOnlyMemory<byte>> Keys { get; init; }

        // Salt used within [decoded]key to encode Data
        public required ReadOnlyMemory<byte> Salt { get; init; }

        // The payload, symmetrically encoded using a key, available by encoding
        // one of Keys using user's private key. The Salt is required to operate
        // (internally states as IV).
        public required ReadOnlyMemory<byte> Data { get; init; }
    }
}
