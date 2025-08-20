using System.Text.Json.Serialization;

namespace MKW.Core.Storage.JSON
{
    public record class AdminUser : DatabaseUser, IDatabaseAdmin, ISavable
    {
        // Signed Public Keys of each trusted users by admin's private credentials.
        [JsonRequired]
        public List<ReadOnlyMemory<byte>> Trust { get; set; }

        internal AdminUser(MemoryDatabaseSession host) : base(new Guid(), host)
        {
            Trust = new List<ReadOnlyMemory<byte>>();
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

        public IEnumerable<ReadOnlyMemory<byte>> EnumerateTrust()
        {
            return Trust.AsReadOnly();
        }

        public void AddTrust(ReadOnlyMemory<byte> data)
        {
            Trust.Add(data);
        }

        public void DeleteTrust(ReadOnlyMemory<byte> data)
        {
            Trust.RemoveAll(t => t.Span.SequenceEqual(data.Span));
        }
    }
}
