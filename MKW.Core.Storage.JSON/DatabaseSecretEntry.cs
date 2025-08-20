using System.Text.Json.Serialization;

namespace MKW.Core.Storage.JSON
{
    public record class DatabaseSecretEntry : IDatabaseEntry, ISavable
    {
        private readonly MemoryDatabaseSession? host;

        internal DatabaseSecretEntry(Guid id, MemoryDatabaseSession host)
            : this(id)
        {
            this.host = host;
        }

        [JsonConstructor]
        internal DatabaseSecretEntry()
        {
            Keys = new Dictionary<Guid, Memory<byte>>();
        }

        public DatabaseSecretEntry(Guid id)
            : this()
        {
            Id = id;
        }

        [JsonIgnore]
        public Guid Id { get; }

        [JsonRequired]
        public IDictionary<Guid, Memory<byte>> Keys { get; set; }

        [JsonRequired]
        public Memory<byte> Salt { get; set; }

        [JsonRequired]
        public Memory<byte> Data { get; set; }

        public void Save()
        {
            if (host == null)
            {
                throw new InvalidOperationException();
            }

            host.Database.Entries[Id] = this;
            host.Save();
        }

        public void CopyFrom(DatabaseSecretEntry other)
        {
            Keys = other.Keys;
            Salt = other.Salt;
            Data = other.Data;
        }
    }
}
