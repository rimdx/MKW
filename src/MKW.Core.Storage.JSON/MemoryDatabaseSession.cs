using MKW.Core.Storage.JSON.Interface;
using MKW.Core.Storage.JSON.Types;

namespace MKW.Core.Storage.JSON
{
    public class MemoryDatabaseSession : IDatabase, IDisposable
    {
        internal readonly JSONDatabase Database;

        public MemoryDatabaseSession()
        {
            Database = new JSONDatabase();
        }

        internal MemoryDatabaseSession(JSONDatabase database)
        {
            Database = database;
        }

        public IDatabaseUser CreateUser(UserId id)
        {
            if (id.IsAdmin)
            {
                if (Database.Admin == null)
                {
                    return new DatabaseAdminUser(this);
                }
                else
                {
                    throw new Exception("Admin user already exists.");
                }
            }
            else
            {
                if (!Database.Users.ContainsKey(id.GetGuid()))
                {
                    return new DatabaseUser(id, this);
                }
                else
                {
                    throw new Exception("User already exists.");
                }
            }
        }

        public IDatabaseUser OpenUser(UserId id, bool readOnly)
        {
            if (id.IsAdmin)
            {
                if (Database.Admin != null)
                {
                    DatabaseUser result = new DatabaseAdminUser(this);
                    result.CopyFrom(Database.Admin);
                    return result;
                }
                else
                {
                    throw new Exception("Admin user does not exist.");
                }
            }
            else
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
        }

        public bool DeleteUser(UserId id)
        {
            bool result = Database.Users.Remove(id.GetGuid());
            Save();
            return result;
        }

        public bool HasUser(UserId id)
        {
            return Database.Users.ContainsKey(id.GetGuid());
        }

        public IEnumerable<IDatabaseUser> EnumerateUsers()
        {
            yield return OpenUser(UserId.Admin(), true);

            foreach (KeyValuePair<Guid, JSONDatabaseUser> item in Database.Users)
            {
                DatabaseUser result = new DatabaseUser(UserId.FromGuid(item.Key));
                result.CopyFrom(item.Value);
                yield return result;
            }
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
            bool result = Database.Entries.Remove(id.GetGuid());
            Save();
            return result;
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
