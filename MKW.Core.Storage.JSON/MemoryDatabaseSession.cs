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

        public IDatabaseUser CreateUser(Guid id)
        {
            if (Database.Users.ContainsKey(id))
            {
                throw new Exception("User already exists.");
            }

            return new DatabaseUser(id, this);
        }

        public IDatabaseUser OpenUser(Guid id, bool readOnly)
        {
            DatabaseUser result = readOnly ? new DatabaseUser(id)
                                           : new DatabaseUser(id, this);

            if (Database.Users.TryGetValue(id, out JSONDatabaseUser? user))
            {
                result.CopyFrom(user);
                return result;
            }
            else
            {
                throw new Exception("User doesn't exist.");
            }
        }

        public bool DeleteUser(Guid id)
        {
            return Database.Users.Remove(id);
        }

        public bool HasUser(Guid id)
        {
            return Database.Users.ContainsKey(id);
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

        public IDatabaseEntry CreateEntry(Guid id)
        {
            if (Database.Entries.ContainsKey(id))
            {
                throw new Exception("Entry already exists.");
            }

            return new DatabaseSecretEntry(id, this);
        }

        public IDatabaseEntry OpenEntry(Guid id, bool readOnly)
        {
            DatabaseSecretEntry result = readOnly ? new DatabaseSecretEntry(id)
                                                  : new DatabaseSecretEntry(id, this);

            if (Database.Entries.TryGetValue(id, out JSONDatabaseSecretEntry? entry))
            {
                result.CopyFrom(entry);
                return result;
            }
            else
            {
                throw new Exception("Entry doesn't exist.");
            }
        }

        public bool DeleteEntry(Guid id)
        {
            return Database.Entries.Remove(id);
        }

        public bool HasEntry(Guid id)
        {
            return Database.Entries.ContainsKey(id);
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
