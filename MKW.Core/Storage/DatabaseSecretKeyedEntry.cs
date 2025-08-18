namespace MKW.Core.Storage
{
    public record class DatabaseSecretKeyedEntry : DatabaseSecretEntry
    {
        public required Guid Id { get; set; }
    }
}
