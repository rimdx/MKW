using MKW.Core.Storage.JSON.Types;

namespace MKW.Core.Storage.JSON.Interface
{
    public record class DatabaseSecretEntry : IDatabaseEntry, ISavable
    {
        private readonly MemoryDatabaseSession? host;

        internal DatabaseSecretEntry(Guid id, MemoryDatabaseSession host)
            : this(id)
        {
            this.host = host;
        }

        public DatabaseSecretEntry(Guid id)
        {
            Keys = new Dictionary<Guid, ReadOnlyMemory<byte>>();
            Id = id;
        }

        public Guid Id { get; }
        public IDictionary<Guid, ReadOnlyMemory<byte>> Keys { get; set; }
        public ReadOnlyMemory<byte> Salt { get; set; }
        public ReadOnlyMemory<byte> Data { get; set; }

        public void Save()
        {
            if (host == null)
            {
                throw new InvalidOperationException();
            }

            host.Database.Entries[Id] = AsJSONObject();
            host.Save();
        }

        public void CopyFrom(JSONDatabaseSecretEntry other)
        {
            Keys = other.Keys;
            Salt = other.Salt;
            Data = other.Data;
        }

        public JSONDatabaseSecretEntry AsJSONObject()
        {
            return new JSONDatabaseSecretEntry
            {
                Keys = Keys,
                Salt = Salt,
                Data = Data
            };
        }
    }
}
