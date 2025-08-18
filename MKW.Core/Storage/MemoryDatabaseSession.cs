
namespace MKW.Core.Storage
{
    public class MemoryDatabaseSession : IDatabaseSession, IDisposable
    {
        public readonly Database Database;

        public MemoryDatabaseSession()
        {
            Database = new Database();
        }

        public MemoryDatabaseSession(Database database)
        {
            Database = database;
        }

        public void AddUser(Guid id, DatabaseUser user)
        {
            Database.Users.Add(user);
            Save();
        }

        public DatabaseUser GetUser(Guid id)
        {
            return Database.Users.First(u => u.Id == id);
        }

        public IEnumerable<DatabaseUser> EnumerateUsers()
        {
            return Database.Users;
        }

        public void UpdateEntry(Guid id, DatabaseSecretEntry? entry)
        {
            if (entry == null)
            {
                Database.Entries.Remove(id);
            }
            else
            {
                Database.Entries[id] = entry;
            }

            Save();
        }

        public DatabaseSecretEntry? QueryEntry(Guid id)
        {
            if (Database.Entries.TryGetValue(id, out DatabaseSecretEntry? entry))
            {
                return entry;
            }
            else
            {
                return null;
            }
        }

        public virtual void Save()
        {
        }

        public virtual void Dispose()
        {
        }
    }
}
