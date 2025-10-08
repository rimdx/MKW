namespace MKW.Core.Serialization
{
    public sealed record class BackupEntry
    {
        public IReadOnlyDictionary<string, string> Fields { get; }

        public BackupEntry(IReadOnlyDictionary<string, string> fields)
        {
            Fields = fields;
        }
    }
}
