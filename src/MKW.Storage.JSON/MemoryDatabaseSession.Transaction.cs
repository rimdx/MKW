// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Storage.Exceptions;

namespace MKW.Storage.JSON
{
    public partial class MemoryDatabaseSession
    {
        private sealed class Transaction
            : SnapshotBase
            , IDatabaseNG.ISnapshot
            , IDatabaseNG.ITransaction
            , IDisposable
        {
            private readonly MemoryDatabaseSession database;

            public Transaction(MemoryDatabaseSession database)
            {
                this.database = database;
            }

            protected override JSONDatabase Database => database.Database;

            // Entry
            public void CreateEntry(DatabaseEntry entry)
            {
                if (Database.Entries.ContainsKey(entry.Id.GetStringLegacy()))
                {
                    throw new EntryAlreadyExistsException();
                }
                else
                {
                    Database.Entries[entry.Id.GetStringLegacy()] = JSONDatabaseSecretEntry.Serialize(entry);
                }
            }

            public void UpdateEntry(DatabaseEntry entry)
            {
                if (Database.Entries.ContainsKey(entry.Id.GetStringLegacy()))
                {
                    Database.Entries[entry.Id.GetStringLegacy()] = JSONDatabaseSecretEntry.Serialize(entry);
                }
                else
                {
                    throw new EntryDoesNotExistException();
                }
            }

            public bool DeleteEntry(EntryId id)
            {
                return Database.Entries.Remove(id.GetStringLegacy());
            }

            // User
            public void CreateUser(DatabaseUser user)
            {
                if (user.Id.IsAdmin)
                {
                    if (Database.Admin == null)
                    {
                        Database.Admin = JSONDatabaseUser.Serialize(user);
                    }
                    else
                    {
                        throw new AdminAlreadyExistsException();
                    }
                }
                else
                {
                    if (!Database.Users.ContainsKey(user.Id.GetStringLegacy()))
                    {
                        Database.Users.Add(user.Id.GetStringLegacy(), JSONDatabaseUser.Serialize(user));
                    }
                    else
                    {
                        throw new UserAlreadyExistsException();
                    }
                }
            }

            public void UpdateUser(DatabaseUser user)
            {
                if (user.Id.IsAdmin)
                {
                    if (Database.Admin != null)
                    {
                        Database.Admin = JSONDatabaseUser.Serialize(user);
                    }
                    else
                    {
                        throw new AdminDoesNotExistException();
                    }
                }
                else
                {
                    if (Database.Users.ContainsKey(user.Id.GetStringLegacy()))
                    {
                        Database.Users[user.Id.GetStringLegacy()] = JSONDatabaseUser.Serialize(user);
                    }
                    else
                    {
                        throw new UserDoesNotExistException();
                    }
                }
            }

            public bool DeleteUser(UserId id)
            {
                return Database.Users.Remove(id.GetStringLegacy());
            }

            // Misc
            public void Commit()
            {
                database.Save();

                // todo:
            }

            public void Dispose()
            {
                // todo:
                // no-op
            }
        }
    }
}
