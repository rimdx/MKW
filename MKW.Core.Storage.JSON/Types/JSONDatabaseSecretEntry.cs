namespace MKW.Core.Storage.JSON.Types
{
    public record class JSONDatabaseSecretEntry
    {
        public required IDictionary<Guid, ReadOnlyMemory<byte>> Keys { get; set; }

        public required ReadOnlyMemory<byte> Salt { get; set; }

        public required ReadOnlyMemory<byte> Data { get; set; }
    }
}
