using System.Text.Json.Serialization;

namespace MKW.Core.Storage.JSON
{
    internal class JSONDatabase
    {
        public JSONDatabaseUser? Admin { get; set; }

        [JsonRequired]
        public IDictionary<Guid, JSONDatabaseUser> Users { get; init; }

        // EntryId -> Entry
        [JsonRequired]
        public IDictionary<Guid, JSONDatabaseSecretEntry> Entries { get; init; }

        public JSONDatabase()
        {
            Users = new Dictionary<Guid, JSONDatabaseUser>();
            Entries = new Dictionary<Guid, JSONDatabaseSecretEntry>();
        }
    }
}
