namespace MKW.Storage.MKPG
{
    internal sealed class BlobStorageMemory
    {
        private readonly Dictionary<BlobId, BlobEntry> entries;

        public BlobStorageMemory()
        {
            entries = [];
        }

        public void Create(BlobId id, BlobEntry entry)
        {
            return entries[id] = entry;
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

        public IEnumerable<BlobId> Enumerate()
        {
            return entries.Keys;
        }
    }
}
