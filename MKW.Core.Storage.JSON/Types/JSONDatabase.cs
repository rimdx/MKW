using System.Text.Json.Serialization;

namespace MKW.Core.Storage.JSON.Types
{
    public class JSONDatabase
    {
        public JSONDatabaseAdminUser? Admin { get; set; }

        [JsonRequired]
        public IDictionary<Guid, JSONDatabaseUser> Users { get; set; }

        // EntryId -> Entry
        [JsonRequired]
        public IDictionary<Guid, JSONDatabaseSecretEntry> Entries { get; set; }

        public JSONDatabase()
        {
            Users = new Dictionary<Guid, JSONDatabaseUser>();
            Entries = new Dictionary<Guid, JSONDatabaseSecretEntry>();
        }
    }
}
