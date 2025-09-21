using MKW.Core.Storage.JSON.Types;

namespace MKW.Core.Storage.JSON.Interface
{
    internal record class DatabaseSecretEntry : IDatabaseEntry, ISavable
    {
        private readonly MemoryDatabaseSession? host;

        internal DatabaseSecretEntry(EntryId id, MemoryDatabaseSession host)
            : this(id)
        {
            this.host = host;
        }

        public DatabaseSecretEntry(EntryId id)
        {
            Keys = new Dictionary<UserId, ReadOnlyMemory<byte>>();
            Id = id;
        }

        public EntryId Id { get; }
        public IDictionary<UserId, ReadOnlyMemory<byte>> Keys { get; set; }
        public ReadOnlyMemory<byte> Salt { get; set; }
        public ReadOnlyMemory<byte> Data { get; set; }

        public void Save()
        {
            if (host == null)
            {
                throw new InvalidOperationException();
            }

            host.Database.Entries[Id.GetGuid()] = AsJSONObject();
            host.Save();
        }

        public void CopyFrom(JSONDatabaseSecretEntry other)
        {
            Dictionary<UserId, ReadOnlyMemory<byte>> keys = [];

            foreach (KeyValuePair<Guid, ReadOnlyMemory<byte>> pair in other.Keys)
            {
                keys.Add(UserId.FromGuid(pair.Key), pair.Value);
            }

            Keys = keys;
            Salt = other.Salt;
            Data = other.Data;
        }

        public JSONDatabaseSecretEntry AsJSONObject()
        {
            Dictionary<Guid, ReadOnlyMemory<byte>> keys = [];

            foreach (KeyValuePair<UserId, ReadOnlyMemory<byte>> pair in Keys)
            {
                keys.Add(pair.Key.GetGuid(), pair.Value);
            }

            return new JSONDatabaseSecretEntry
            {
                Keys = keys,
                Salt = Salt,
                Data = Data
            };
        }
    }
}
