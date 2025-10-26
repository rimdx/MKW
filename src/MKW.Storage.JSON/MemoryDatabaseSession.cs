// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Cryptography;

namespace MKW.Storage.JSON
{
    public partial class MemoryDatabaseSession : IDatabase, IDatabaseNG, IDisposable
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
            return CreateSnapshotInternal().OpenUser(id);
        }

        public void CreateUser(UserId id, DatabaseUser user)
        {
            using (Transaction transaction = BeginTransactionInternal())
            {
                transaction.CreateUser(user);
                transaction.Commit();
            }
        }

        public void UpdateUser(UserId id, DatabaseUser user)
        {
            using (Transaction transaction = BeginTransactionInternal())
            {
                transaction.UpdateUser(user);
                transaction.Commit();
            }
        }

        public bool DeleteUser(UserId id)
        {
            bool result;
            using (Transaction transaction = BeginTransactionInternal())
            {
                result = transaction.DeleteUser(id);
                transaction.Commit();
            }

            return result;
        }

        public bool HasUser(UserId id)
        {
            Snapshot snapshot = CreateSnapshotInternal();

            return snapshot.HasUser(id);
        }

        public IEnumerable<DatabaseUser> EnumerateUsers()
        {
            Snapshot snapshot = CreateSnapshotInternal();

            foreach (DatabaseUser user in snapshot.EnumerateUsers())
            {
                yield return user;
            }
        }

        // Entry

        public void CreateEntry(EntryId id, DatabaseEntry entry)
        {
            using (Transaction transaction = BeginTransactionInternal())
            {
                transaction.CreateEntry(entry);
                transaction.Commit();
            }
        }

        public void UpdateEntry(EntryId id, DatabaseEntry entry)
        {
            using (Transaction transaction = BeginTransactionInternal())
            {
                transaction.UpdateEntry(entry);
                transaction.Commit();
            }
        }

        public DatabaseEntry OpenEntry(EntryId id)
        {
            Snapshot snapshot = CreateSnapshotInternal();

            return snapshot.OpenEntry(id);
        }

        public bool DeleteEntry(EntryId id)
        {
            bool result;
            using (Transaction transaction = BeginTransactionInternal())
            {
                result = transaction.DeleteEntry(id);
                transaction.Commit();
            }

            return result;
        }

        public bool HasEntry(EntryId id)
        {
            Snapshot snapshot = CreateSnapshotInternal();

            return snapshot.HasEntry(id);
        }

        public IEnumerable<DatabaseEntry> EnumerateEntries()
        {
            Snapshot snapshot = CreateSnapshotInternal();

            foreach (DatabaseEntry entry in snapshot.EnumerateEntries())
            {
                yield return entry;
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
        public IDatabaseNG.ITransaction BeginTransaction()
        {
            return BeginTransactionInternal();
        }

        public IDatabaseNG.ISnapshot CreateSnapshot()
        {
            return CreateSnapshotInternal();
        }

        private Transaction BeginTransactionInternal()
        {
            return new Transaction(this);
        }

        private Snapshot CreateSnapshotInternal()
        {
            return new Snapshot(Database);
        }
    }
}
