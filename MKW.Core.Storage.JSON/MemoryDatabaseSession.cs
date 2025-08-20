
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

        public IDatabaseUser OpenUser(Guid id, DatabaseOpenMode mode, out bool created)
        {
            DatabaseUser result = (mode == DatabaseOpenMode.ReadOnly) ? new DatabaseUser(id)
                                                                      : new DatabaseUser(id, this);
            created = false;

            if (Database.Users.TryGetValue(id, out DatabaseUser? user))
            {
                result.CopyFrom(user);
            }
            else
            {
                switch (mode)
                {
                    case DatabaseOpenMode.ReadOnly:
                    case DatabaseOpenMode.Open:
                        throw new Exception("User doesn't exist.");

                    case DatabaseOpenMode.OpenOrCreate:
                        created = true;
                        break;
                }
            }

            return result;
        }

        public bool DeleteUser(Guid id)
        {
            return Database.Users.Remove(id);
        }

        public IEnumerable<IDatabaseUser> EnumerateUsers()
        {
            foreach (var item in Database.Users)
            {
                var result = new DatabaseUser(item.Key);
                result.CopyFrom(item.Value);
                yield return result;
            }
        }

        // Admin

        public IDatabaseAdmin OpenAdmin()
        {
            if (Database.Admin == null)
            {
                return new AdminUser(this);
            }
            else
            {
                return Database.Admin;
            }
        }

        public IDatabaseEntry OpenEntry(Guid id, DatabaseOpenMode mode, out bool created)
        {
            DatabaseSecretEntry result = (mode == DatabaseOpenMode.ReadOnly) ? new DatabaseSecretEntry(id)
                                                                             : new DatabaseSecretEntry(id, this);
            created = false;

            if (Database.Entries.TryGetValue(id, out DatabaseSecretEntry? entry))
            {
                result.CopyFrom(entry);
            }
            else
            {
                switch (mode)
                {
                    case DatabaseOpenMode.ReadOnly:
                    case DatabaseOpenMode.Open:
                        throw new Exception("Entry doesn't exist.");

                    case DatabaseOpenMode.OpenOrCreate:
                        created = true;
                        break;
                }
            }

            return result;
        }

        public bool DeleteEntry(Guid id)
        {
            return Database.Entries.Remove(id);
        }

        public IEnumerable<IDatabaseEntry> EnumerateEntries()
        {
            foreach (var item in Database.Entries)
            {
                DatabaseSecretEntry result = new DatabaseSecretEntry(item.Key);
                result.CopyFrom(item.Value);
                yield return result;
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
