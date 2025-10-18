namespace MKW.Storage.MKPG
{
    internal sealed class BlobStorageMemory : IBlobStorage
    {
        private readonly Dictionary<BlobId, BlobEntry> entries;

        public BlobStorageMemory()
        {
            entries = [];
        }

        public void Create(BlobEntry entry)
        {
            entries[entry.Id] = entry;
        }

        public BlobEntry Open(BlobId id)
        {
            return entries[id];
        }

        public bool Delete(BlobId id)
        {
            return entries.Remove(id);
        }

        public bool Exists(BlobId id)
        {
            return entries.ContainsKey(id);
        }

        public IEnumerable<BlobEntry> Enumerate()
        {
            return entries.Values;
        }
    }
}
