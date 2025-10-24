// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Cryptography;
using MKW.Storage.Exceptions;
using System.Text.Json;

namespace MKW.Storage.JSON
{
    public partial class MemoryDatabaseSession : IDatabase, IDatabase3, IDisposable
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
                    throw new AdminAlreadyExistsException();
                }
            }
            else
            {
                if (!Database.Users.ContainsKey(id.GetStringLegacy()))
                {
                    Database.Users.Add(id.GetStringLegacy(), JSONDatabaseUser.Serialize(user));
                }
                else
                {
                    throw new UserAlreadyExistsException();
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
                    throw new AdminDoesNotExistException();
                }
            }
            else
            {
                if (Database.Users.ContainsKey(id.GetStringLegacy()))
                {
                    Database.Users[id.GetStringLegacy()] = JSONDatabaseUser.Serialize(user);
                }
                else
                {
                    throw new UserDoesNotExistException();
                }
            }

            Save();
        }

        public bool DeleteUser(UserId id)
        {
            bool result = Database.Users.Remove(id.GetStringLegacy());
            Save();
            return result;
        }

        public bool HasUser(UserId id)
        {
            return Database.Users.ContainsKey(id.GetStringLegacy());
        }

        public IEnumerable<DatabaseUser> EnumerateUsers()
        {
            yield return OpenUser(UserId.Admin());

            foreach (KeyValuePair<string, JSONDatabaseUser> item in Database.Users)
            {
                yield return JSONDatabaseUser.Deserialize(UserId.FromStringLegacy(item.Key),
                                                          item.Value);
            }
        }

        // Entry

        public void CreateEntry(EntryId id, DatabaseEntry entry)
        {
            if (Database.Entries.ContainsKey(id.GetStringLegacy()))
            {
                throw new EntryAlreadyExistsException();
            }
            else
            {
                Database.Entries[id.GetStringLegacy()] = JSONDatabaseSecretEntry.Serialize(entry);
            }

            Save();
        }

        public void UpdateEntry(EntryId id, DatabaseEntry entry)
        {
            if (Database.Entries.ContainsKey(id.GetStringLegacy()))
            {
                Database.Entries[id.GetStringLegacy()] = JSONDatabaseSecretEntry.Serialize(entry);
            }
            else
            {
                throw new EntryDoesNotExistException();
            }

            Save();
        }

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

        public bool DeleteEntry(EntryId id)
        {
            bool result = Database.Entries.Remove(id.GetStringLegacy());
            Save();
            return result;
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

        // Trust Signatures
        public void AddTrustSignature(DatabaseTrustSignature signature)
        {
            string id = signature.Id.GetStringLegacy();

            Database.Users[id] = Database.Users[id] with
            {
                AdminSignature = signature.SignatureBytes,
            };
        }

        public void DeleteTrustSignature(DatabaseTrustSignature signature)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DatabaseTrustSignature> EnumerateTrustSignatures()
        {
            foreach (KeyValuePair<string, JSONDatabaseUser> user in Database.Users)
            {
                yield return new DatabaseTrustSignature
                {
                    Id = UserId.FromStringLegacy(user.Key),
                    SignatureBytes = user.Value.AdminSignature,
                };
            }
        }

        // Misc
        public DatabaseConfiguration GetConfiguration()
        {
            return new DatabaseConfiguration
            {
                PreferredSymmetricAlgorithm = CommonCryptographyAlgorithms.Aes128Gcm,
                PreferredPublicKeyAlgorithm = CommonCryptographyAlgorithms.Rsa2048,
                PreferredStringToKeyAlgorithm = CommonCryptographyAlgorithms.Pbkdf2,
            };
        }

        public ReadOnlyMemory<byte> SerializeProtectedData(DatabaseUserProtectedData obj)
        {
            return JSONDatabaseUserProtectedData.Serialize(obj);
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

        // IDatabase3
        public IDatabase3.ITransaction BeginTransaction()
        {
            return new Transaction(this);
        }

        public IDatabase3.ISnapshot CreateSnapshot()
        {
            return new Snapshot(Database);
        }
    }
}
