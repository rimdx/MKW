// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Storage.Exceptions;

namespace MKW.Storage.JSON
{
    public partial class MemoryDatabaseSession
    {
        private abstract class SnapshotBase
            : SerializerBase
            , IDatabaseSerializer
            , IDatabaseNG.ISnapshot
        {
            // readonly
            protected abstract JSONDatabase Database { get; }

            // User
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
                        throw new AdminDoesNotExistException();
                    }
                }
                else
                {
                    if (Database.Users.TryGetValue(id.GetStringLegacy(), out JSONDatabaseUser? user))
                    {
                        return JSONDatabaseUser.Deserialize(id, user);
                    }
                    else
                    {
                        throw new UserDoesNotExistException();
                    }
                }
            }

            public bool HasUser(UserId id)
            {
                return Database.Users.ContainsKey(id.GetStringLegacy());
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

                foreach (KeyValuePair<string, JSONDatabaseUser> item in Database.Users)
                {
                    yield return JSONDatabaseUser.Deserialize(UserId.FromStringLegacy(item.Key),
                                                              item.Value);
                }
            }

            // Entry
            public DatabaseEntry OpenEntry(EntryId id)
            {
                if (Database.Entries.ContainsKey(id.GetStringLegacy()))
                {
                    return JSONDatabaseSecretEntry.Deserialize(id, Database.Entries[id.GetStringLegacy()]);
                }
                else
                {
                    throw new EntryDoesNotExistException();
                }
            }

            public bool HasEntry(EntryId id)
            {
                return Database.Entries.ContainsKey(id.GetStringLegacy());
            }

            public IEnumerable<DatabaseEntry> EnumerateEntries()
            {
                foreach (KeyValuePair<string, JSONDatabaseSecretEntry> item in Database.Entries)
                {
                    yield return JSONDatabaseSecretEntry.Deserialize(EntryId.FromStringLegacy(item.Key), item.Value);
                }
            }
        }
    }
}
