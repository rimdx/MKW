namespace MKW.Core.Storage.JSON.Types
{
    public record class JSONDatabaseSecretEntry
    {
        public required IDictionary<Guid, ReadOnlyMemory<byte>> Keys { get; init; }

        public required ReadOnlyMemory<byte> Salt { get; init; }

        public required ReadOnlyMemory<byte> Data { get; init; }
    }
}
