using System.Text.Json.Serialization;

namespace MKW.Core.Storage
{
    public class Database
    {
        public AdminUser? Admin { get; set; }

        [JsonRequired]
        public IDictionary<Guid, DatabaseUser> Users { get; set; }

        // EntryId -> Entry
        [JsonRequired]
        public IDictionary<Guid, DatabaseSecretEntry> Entries { get; set; }

        public Database()
        {
            Users = new Dictionary<Guid, DatabaseUser>();
            Entries = new Dictionary<Guid, DatabaseSecretEntry>();
        }
    }
}
