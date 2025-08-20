namespace MKW.Core.Storage
{
    public interface IDatabaseEntry : ISavable
    {
        Guid Id { get; }

        // User -> Payload
        IDictionary<Guid, Memory<byte>> Keys { get; set; }

        // Salt used within [decoded]key to encode Data
        Memory<byte> Salt { get; set; }

        // The payload, symmetrically encoded using a key, available by encoding
        // one of Keys using user's private key. The Salt is required to operate
        // (internally states as IV).
        Memory<byte> Data { get; set; }
    }
}
