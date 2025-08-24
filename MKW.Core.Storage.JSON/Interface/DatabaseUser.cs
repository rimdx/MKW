using MKW.Core.Storage.JSON.Types;

namespace MKW.Core.Storage.JSON.Interface
{
    internal record class DatabaseUser : IDatabaseUser, ISavable
    {
        protected readonly MemoryDatabaseSession? host;

        internal DatabaseUser(UserId id, MemoryDatabaseSession host)
            : this(id)
        {
            this.host = host;
        }

        public DatabaseUser(UserId id)
        {
            Id = id;
        }

        public UserId Id { get; }
        public ReadOnlyMemory<byte> Salt { get; set; }
        public ReadOnlyMemory<byte> PublicKey { get; set; }
        public ReadOnlyMemory<byte> PrivateKey { get; set; }

        public void Save()
        {
            if (host == null)
            {
                throw new InvalidOperationException();
            }

            host.Database.Users[Id.GetGuid()] = AsJSONObject();
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
