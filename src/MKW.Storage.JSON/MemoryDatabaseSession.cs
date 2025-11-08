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
