using System.Text.Json.Serialization;

namespace MKW.Storage.JSON
{
    internal sealed class JSONDatabase
    {
        public JSONDatabaseUser? Admin { get; set; }

        [JsonRequired]
        public IDictionary<Guid, JSONDatabaseUser> Users { get; init; }

        // EntryId -> Entry
        [JsonRequired]
        public IDictionary<string, JSONDatabaseSecretEntry> Entries { get; init; }

        public JSONDatabase()
        {
            Users = new Dictionary<Guid, JSONDatabaseUser>();
            Entries = new Dictionary<string, JSONDatabaseSecretEntry>();
        }
    }
}
