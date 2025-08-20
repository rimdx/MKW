using System.Text.Json.Serialization;

namespace MKW.Core.Storage
{
    public record class AdminUser : DatabaseUser, IDatabaseAdmin, ISavable
    {
        // Signed Public Keys of each trusted users by admin's private credentials.
        // FIXME: comparable signature instead of byte[]!!!
        [JsonRequired]
        public HashSet<Memory<byte>> Trust { get; set; }

        internal AdminUser(MemoryDatabaseSession host) : base(new Guid(), host)
        {
            Trust = new HashSet<Memory<byte>>();
        }

        [JsonConstructor]
        internal AdminUser() : base()
        {
            Trust = null!;
        }

        public override void Save()
        {
            if (host == null)
            {
                throw new InvalidOperationException();
            }

            host.Database.Admin = this;
            host.Save();
        }
    }
}
