using MKW.Core.Storage.JSON.Types;

namespace MKW.Core.Storage.JSON.Interface
{
    public record class DatabaseAdminUser : IDatabaseAdmin, ISavable
    {
        protected readonly MemoryDatabaseSession? host;
        private List<ReadOnlyMemory<byte>> trust = new List<ReadOnlyMemory<byte>>();

        internal DatabaseAdminUser(MemoryDatabaseSession host)
        {
            this.host = host;
        }

        public DatabaseAdminUser()
        {
        }

        public Guid Id => Guid.Empty;
        public ReadOnlyMemory<byte> Salt { get; set; }
        public ReadOnlyMemory<byte> PublicKey { get; set; }
        public ReadOnlyMemory<byte> PrivateKey { get; set; }

        public void Save()
        {
            if (host == null)
            {
                throw new InvalidOperationException();
            }

            host.Database.Admin = AsJSONObject();
            host.Save();
        }

        public void CopyFrom(JSONDatabaseUser obj)
        {
            PublicKey = obj.PublicKey;
            PrivateKey = obj.PrivateKey;
            Salt = obj.Salt;
        }


        public IEnumerable<ReadOnlyMemory<byte>> EnumerateTrust()
        {
            return trust.AsReadOnly();
        }

        public void AddTrust(ReadOnlyMemory<byte> data)
        {
            trust.Add(data);
        }

        public void DeleteTrust(ReadOnlyMemory<byte> data)
        {
            trust.RemoveAll(t => t.Span.SequenceEqual(data.Span));
        }

        public void CopyFrom(JSONDatabaseAdminUser obj)
        {
            PublicKey = obj.PublicKey;
            PrivateKey = obj.PrivateKey;
            Salt = obj.Salt;
            trust = obj.Trust.ToList();
        }

        public JSONDatabaseAdminUser AsJSONObject()
        {
            return new JSONDatabaseAdminUser
            {
                PublicKey = PublicKey,
                PrivateKey = PrivateKey,
                Salt = Salt,
                Trust = trust.ToList(),
            };
        }
    }
}
