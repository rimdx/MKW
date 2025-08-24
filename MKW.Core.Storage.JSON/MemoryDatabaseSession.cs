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

        public IDatabaseUser CreateUser(UserId id)
        {
            if (Database.Users.ContainsKey(id.GetGuid()))
            {
                throw new Exception("User already exists.");
            }

            return new DatabaseUser(id, this);
        }

        public IDatabaseUser OpenUser(UserId id, bool readOnly)
        {
            DatabaseUser result = readOnly ? new DatabaseUser(id)
                                           : new DatabaseUser(id, this);

            if (Database.Users.TryGetValue(id.GetGuid(), out JSONDatabaseUser? user))
            {
                result.CopyFrom(user);
                return result;
            }
            else
            {
                throw new Exception("User doesn't exist.");
            }
        }

        public bool DeleteUser(UserId id)
        {
            return Database.Users.Remove(id.GetGuid());
        }

        public bool HasUser(UserId id)
        {
            return Database.Users.ContainsKey(id.GetGuid());
        }

        public IEnumerable<IDatabaseUser> EnumerateUsers()
        {
            foreach (KeyValuePair<Guid, JSONDatabaseUser> item in Database.Users)
            {
                DatabaseUser result = new DatabaseUser(UserId.FromGuid(item.Key));
                result.CopyFrom(item.Value);
                yield return result;
            }
        }

        // Admin

        public IDatabaseAdmin OpenAdmin(bool readOnly)
        {
            if (Database.Admin == null)
            {
                throw new Exception("Admin user does not exist.");
            }

            DatabaseAdminUser result = new DatabaseAdminUser(this);

            result.CopyFrom(Database.Admin);

            return result;
        }

        public IDatabaseAdmin CreateAdmin()
        {
            if (Database.Admin != null)
            {
                throw new Exception("Admin user already exists.");
            }

            return new DatabaseAdminUser(this);
        }

        // Entry

        public IDatabaseEntry CreateEntry(EntryId id)
        {
            if (Database.Entries.ContainsKey(id.GetGuid()))
            {
                throw new Exception("Entry already exists.");
            }

            return new DatabaseSecretEntry(id, this);
        }

        public IDatabaseEntry OpenEntry(EntryId id, bool readOnly)
        {
            DatabaseSecretEntry result = readOnly ? new DatabaseSecretEntry(id)
                                                  : new DatabaseSecretEntry(id, this);

            if (Database.Entries.TryGetValue(id.GetGuid(), out JSONDatabaseSecretEntry? entry))
            {
                result.CopyFrom(entry);
                return result;
            }
            else
            {
                throw new Exception("Entry doesn't exist.");
            }
        }

        public bool DeleteEntry(EntryId id)
        {
            return Database.Entries.Remove(id.GetGuid());
        }

        public bool HasEntry(EntryId id)
        {
            return Database.Entries.ContainsKey(id.GetGuid());
        }

        public IEnumerable<IDatabaseEntry> EnumerateEntries()
        {
            foreach (KeyValuePair<Guid, JSONDatabaseSecretEntry> item in Database.Entries)
            {
                DatabaseSecretEntry result = new DatabaseSecretEntry(EntryId.FromGuid(item.Key));
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
