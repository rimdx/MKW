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
        public SignedPayload PublicKey { get; set; }
        public SecretPayload PrivateKey { get; set; }

        public SignedPayload Metadata { get; set; }

        public ReadOnlyMemory<byte> AdminSignature { get; set; }

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
            PublicKey = new SignedPayload(obj.PublicKey, obj.AdminTrustSignature);

            PrivateKey = new SecretPayload(obj.PrivateKey);
            Salt = obj.Salt;

            Metadata = new SignedPayload(obj.Metadata, obj.MetadataAdminSignature);

            AdminSignature = obj.AdminSignature;
        }

        public JSONDatabaseUser AsJSONObject()
        {
            return new JSONDatabaseUser
            {
                PublicKey = PublicKey.Payload,
                AdminTrustSignature = PublicKey.Signature,

                PrivateKey = PrivateKey.EncryptedPayload,
                Salt = Salt,
                Metadata = Metadata.Payload,
                MetadataAdminSignature = Metadata.Signature,
                AdminSignature = AdminSignature,
            };
        }
    }
}
