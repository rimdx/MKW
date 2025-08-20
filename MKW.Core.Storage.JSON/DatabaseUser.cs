using System.Text.Json.Serialization;

namespace MKW.Core.Storage.JSON
{
    public record class DatabaseUser : IDatabaseUser, ISavable
    {
        protected readonly MemoryDatabaseSession? host;

        internal DatabaseUser(Guid id, MemoryDatabaseSession host)
            : this(id)
        {
            this.host = host;
        }

        [JsonConstructor]
        internal DatabaseUser()
        {
        }

        public DatabaseUser(Guid id)
        {
            Id = id;
        }

        [JsonIgnore]
        public Guid Id { get; }

        [JsonRequired]
        public ReadOnlyMemory<byte> Salt { get; set; }

        [JsonRequired]
        public ReadOnlyMemory<byte> PublicKey { get; set; }

        [JsonRequired]
        public ReadOnlyMemory<byte> PrivateKey { get; set; }

        public virtual void Save()
        {
            if (host == null)
            {
                throw new InvalidOperationException();
            }

            host.Database.Users[Id] = this;
            host.Save();
        }

        public void CopyFrom(DatabaseUser value)
        {
            PublicKey = value.PublicKey;
            PrivateKey = value.PrivateKey;
            Salt = value.Salt;
        }
    }
}
