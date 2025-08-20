namespace MKW.Core.Client
{
    public record class KeyedEntry
    {
        public required Guid Id { get; init; }
        public required EntryPayload? Payload { get; init; }
    }
}
