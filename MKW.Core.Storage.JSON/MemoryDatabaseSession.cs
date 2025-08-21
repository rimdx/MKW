using MKW.Core.Storage.JSON.Interface;
using MKW.Core.Storage.JSON.Types;

namespace MKW.Core.Storage.JSON
{
    public class MemoryDatabaseSession : IDatabase, IDisposable
    {
        public readonly JSONDatabase Database;

        public MemoryDatabaseSession()
        {
            Database = new JSONDatabase();
        }

        public MemoryDatabaseSession(JSONDatabase database)
        {
            Database = database;
        }

        public IDatabaseUser OpenUser(Guid id, DatabaseOpenMode mode, out bool created)
        {
            DatabaseUser result = mode == DatabaseOpenMode.ReadOnly ? new DatabaseUser(id)
                                                                    : new DatabaseUser(id, this);
            created = false;

            if (Database.Users.TryGetValue(id, out JSONDatabaseUser? user))
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
            foreach (KeyValuePair<Guid, JSONDatabaseUser> item in Database.Users)
            {
                DatabaseUser result = new DatabaseUser(item.Key);
                result.CopyFrom(item.Value);
                yield return result;
            }
        }

        // Admin

        public IDatabaseAdmin OpenAdmin(out bool created)
        {
            DatabaseAdminUser result = new DatabaseAdminUser(this);

            if (Database.Admin == null)
            {
                created = true;
            }
            else
            {
                created = false;
                result.CopyFrom(Database.Admin);
            }

            return result;
        }

        public IDatabaseEntry OpenEntry(Guid id, DatabaseOpenMode mode, out bool created)
        {
            DatabaseSecretEntry result = mode == DatabaseOpenMode.ReadOnly ? new DatabaseSecretEntry(id)
                                                                           : new DatabaseSecretEntry(id, this);
            created = false;

            if (Database.Entries.TryGetValue(id, out JSONDatabaseSecretEntry? entry))
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
            foreach (KeyValuePair<Guid, JSONDatabaseSecretEntry> item in Database.Entries)
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
