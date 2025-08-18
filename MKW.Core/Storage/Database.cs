namespace MKW.Core.Storage
{
    public class Database
    {
        public IList<DatabaseUser> Users { get; set; }

        // EntryId -> Entry
        public IDictionary<Guid, DatabaseSecretEntry> Entries { get; set; }

        public Database()
        {
            Users = new List<DatabaseUser>();
            Entries = new Dictionary<Guid, DatabaseSecretEntry>();
        }
    }
}
