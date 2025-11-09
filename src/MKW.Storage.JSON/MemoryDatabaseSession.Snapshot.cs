// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Storage.Exceptions;

namespace MKW.Storage.JSON
{
    public partial class MemoryDatabaseSession
    {
        private sealed class Snapshot
            : SerializerBase
            , IDatabaseSerializer
            , IDatabaseNG.ISnapshot
        {
            private readonly JSONDatabase database;

            public Snapshot(JSONDatabase database)
            {
                this.database = database;
            }

            // User
            public DatabaseUser OpenUser(UserId id)
            {
                if (id.IsAdmin)
                {
                    if (database.Admin != null)
                    {
                        return JSONDatabaseUser.Deserialize(database, id, database.Admin);
                    }
                    else
                    {
                        throw new AdminDoesNotExistException();
                    }
                }
                else
                {
                    if (database.Users.TryGetValue(id.GetStringLegacy(), out JSONDatabaseUser? user))
                    {
                        return JSONDatabaseUser.Deserialize(database, id, user);
                    }
                    else
                    {
                        throw new UserDoesNotExistException();
                    }
                }
            }

            public bool HasUser(UserId id)
            {
                return database.Users.ContainsKey(id.GetStringLegacy());
            }

            public IEnumerable<DatabaseUser> EnumerateUsers()
            {
                DatabaseUser? admin;
                try
                {
                    admin = OpenUser(UserId.Admin());
                }
                catch (AdminDoesNotExistException)
                {
                    admin = null;
                }

                if (admin != null)
                {
                    yield return admin;
                }

                foreach (KeyValuePair<string, JSONDatabaseUser> item in database.Users)
                {
                    yield return JSONDatabaseUser.Deserialize(database,
                                                              UserId.FromStringLegacy(item.Key),
                                                              item.Value);
                }
            }

            // Entry
            public DatabaseEntry OpenEntry(EntryId id)
            {
                if (database.Entries.ContainsKey(id.GetStringLegacy()))
                {
                    return JSONDatabaseSecretEntry.Deserialize(id, database.Entries[id.GetStringLegacy()]);
                }
                else
                {
                    throw new EntryDoesNotExistException();
                }
            }

            public bool HasEntry(EntryId id)
            {
                return database.Entries.ContainsKey(id.GetStringLegacy());
            }

            public IEnumerable<DatabaseEntry> EnumerateEntries()
            {
                foreach (KeyValuePair<string, JSONDatabaseSecretEntry> item in database.Entries)
                {
                    yield return JSONDatabaseSecretEntry.Deserialize(EntryId.FromStringLegacy(item.Key), item.Value);
                }
            }
        }
    }
}
