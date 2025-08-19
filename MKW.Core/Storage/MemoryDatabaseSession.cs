
namespace MKW.Core.Storage
{
    public class MemoryDatabaseSession : IDatabase, IDisposable
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

        // Admin

        public void UpdateAdmin(AdminUser user)
        {
            Database.Admin = user;
            Save();
        }

        public AdminUser GetAdmin()
        {
            if (Database.Admin == null)
            {
                throw new Exception("Admin user is not set in the database.");
            }

            return Database.Admin;
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

        public IEnumerable<DatabaseSecretKeyedEntry> EnumerateEntries()
        {
            foreach (KeyValuePair<Guid, DatabaseSecretEntry> entry in Database.Entries)
            {
                yield return new DatabaseSecretKeyedEntry
                {
                    Id = entry.Key,
                    Keys = entry.Value.Keys,
                    Data = entry.Value.Data,
                    Salt = entry.Value.Salt
                };
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
