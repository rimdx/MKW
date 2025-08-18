namespace MKW.Core.Storage
{
    public class Database
    {
        public IList<User> Users { get; set; }

        // EntryId -> Entry
        public IDictionary<Guid, Entry> Entries { get; set; }

        public Database()
        {
            Users = new List<User>();
            Entries = new Dictionary<Guid, Entry>();
        }
    }
}
