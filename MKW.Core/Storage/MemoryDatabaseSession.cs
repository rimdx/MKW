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

        public void AddUser(Guid id, User user)
        {
            Database.Users.Add(user);
            Save();
        }

        public User GetUser(Guid id)
        {
            return Database.Users.First(u => u.Id == id);
        }

        public void UpdateEntry(Guid id, Entry? entry)
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

        public virtual void Save()
        {
        }

        public virtual void Dispose()
        {
        }
    }
}
