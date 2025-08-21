using MKW.Core.Storage.JSON.Types;

namespace MKW.Core.Storage.JSON.Interface
{
    public record class DatabaseUser : IDatabaseUser, ISavable
    {
        protected readonly MemoryDatabaseSession? host;

        internal DatabaseUser(Guid id, MemoryDatabaseSession host)
            : this(id)
        {
            this.host = host;
        }

        public DatabaseUser(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
        public ReadOnlyMemory<byte> Salt { get; set; }
        public ReadOnlyMemory<byte> PublicKey { get; set; }
        public ReadOnlyMemory<byte> PrivateKey { get; set; }

        public void Save()
        {
            if (host == null)
            {
                throw new InvalidOperationException();
            }

            host.Database.Users[Id] = AsJSONObject();
            host.Save();
        }

        public void CopyFrom(JSONDatabaseUser obj)
        {
            PublicKey = obj.PublicKey;
            PrivateKey = obj.PrivateKey;
            Salt = obj.Salt;
        }

        public JSONDatabaseUser AsJSONObject()
        {
            return new JSONDatabaseUser
            {
                PublicKey = PublicKey,
                PrivateKey = PrivateKey,
                Salt = Salt
            };
        }
    }
}
