// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;

namespace MKW.Storage.JSON
{
    public class MemoryDatabaseSession : IDatabase, IDisposable
    {
        internal JSONDatabase Database;

        public MemoryDatabaseSession()
        {
            Database = new JSONDatabase();
        }

        internal MemoryDatabaseSession(JSONDatabase database)
        {
            Database = database;
        }

        public DatabaseUser OpenUser(UserId id)
        {
            if (id.IsAdmin)
            {
                if (Database.Admin != null)
                {
                    return JSONDatabaseUser.Deserialize(id, Database.Admin);
                }
                else
                {
                    throw new Exception("Admin user does not exist.");
                }
            }
            else
            {
                if (Database.Users.TryGetValue(id.GetGuid(), out JSONDatabaseUser? user))
                {
                    return JSONDatabaseUser.Deserialize(id, user);
                }
                else
                {
                    throw new Exception("User doesn't exist.");
                }
            }
        }

        public void CreateUser(UserId id, DatabaseUser user)
        {
            if (id.IsAdmin)
            {
                if (Database.Admin == null)
                {
                    Database.Admin = JSONDatabaseUser.Serialize(user);
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
                    Database.Users.Add(id.GetGuid(), JSONDatabaseUser.Serialize(user));
                }
                else
                {
                    throw new Exception("User already exists.");
                }
            }

            Save();
        }

        public void UpdateUser(UserId id, DatabaseUser user)
        {
            if (id.IsAdmin)
            {
                if (Database.Admin != null)
                {
                    Database.Admin = JSONDatabaseUser.Serialize(user);
                }
                else
                {
                    throw new Exception("Admin user does not exist.");
                }
            }
            else
            {
                if (Database.Users.ContainsKey(id.GetGuid()))
                {
                    Database.Users[id.GetGuid()] = JSONDatabaseUser.Serialize(user);
                }
                else
                {
                    throw new Exception("User does not exist.");
                }
            }

            Save();
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

        public IEnumerable<DatabaseUser> EnumerateUsers()
        {
            yield return OpenUser(UserId.Admin());

            foreach (KeyValuePair<Guid, JSONDatabaseUser> item in Database.Users)
            {
                yield return JSONDatabaseUser.Deserialize(UserId.FromGuid(item.Key),
                                                          item.Value);
            }
        }

        // Entry

        public void CreateEntry(EntryId id, DatabaseEntry entry)
        {
            if (Database.Entries.ContainsKey(id.GetString()))
            {
                throw new Exception("Entry already exists.");
            }
            else
            {
                Database.Entries[id.GetString()] = JSONDatabaseSecretEntry.Serialize(entry);
            }

            Save();
        }

        public void UpdateEntry(EntryId id, DatabaseEntry entry)
        {
            if (Database.Entries.ContainsKey(id.GetString()))
            {
                Database.Entries[id.GetString()] = JSONDatabaseSecretEntry.Serialize(entry);
            }
            else
            {
                throw new Exception("Entry does not exist.");
            }

            Save();
        }

        public DatabaseEntry OpenEntry(EntryId id)
        {
            if (Database.Entries.ContainsKey(id.GetString()))
            {
                return JSONDatabaseSecretEntry.Deserialize(id, Database.Entries[id.GetString()]);
            }
            else
            {
                throw new Exception("Entry does not exist.");
            }
        }

        public bool DeleteEntry(EntryId id)
        {
            bool result = Database.Entries.Remove(id.GetString());
            Save();
            return result;
        }

        public bool HasEntry(EntryId id)
        {
            return Database.Entries.ContainsKey(id.GetString());
        }

        public IEnumerable<DatabaseEntry> EnumerateEntries()
        {
            foreach (KeyValuePair<string, JSONDatabaseSecretEntry> item in Database.Entries)
            {
                yield return JSONDatabaseSecretEntry.Deserialize(EntryId.FromString(item.Key), item.Value);
            }
        }

        public virtual void ReloadDatabaseFile()
        {
        }

        public virtual async Task<bool> WaitForDatabaseChangesAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(-1, cancellationToken);
            return false;
        }

        public virtual void Save()
        {
        }

        public virtual void Dispose()
        {
        }
    }
}
