using MKW.Core.Storage.JSON.Types;

namespace MKW.Core.Storage.JSON.Interface
{
    internal record class DatabaseUser : IDatabaseUser, ISavable
    {
        protected readonly MemoryDatabaseSession? host;
        protected List<ReadOnlyMemory<byte>> trust = new List<ReadOnlyMemory<byte>>();

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

        public ReadOnlyMemory<byte> Metadata { get; set; }
        public ReadOnlyMemory<byte> MetadataAdminSignature { get; set; }

        public ReadOnlyMemory<byte> AdminTrustSignature { get; set; }

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

        public virtual void Save()
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
            Metadata = obj.Metadata;
            MetadataAdminSignature = obj.MetadataAdminSignature;
            AdminTrustSignature = obj.AdminTrustSignature;
            trust = [.. obj.Trust];
        }

        public JSONDatabaseUser AsJSONObject()
        {
            return new JSONDatabaseUser
            {
                PublicKey = PublicKey,
                PrivateKey = PrivateKey,
                Salt = Salt,
                Metadata = Metadata,
                MetadataAdminSignature = MetadataAdminSignature,
                AdminTrustSignature = AdminTrustSignature,
                Trust = [.. trust]
            };
        }
    }
}
