// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;

namespace MKW.Storage
{
    public sealed partial class MDatabase
    {
        private sealed class Transaction
            : SnapshotBase
            , IDatabaseSerializer
            , IDatabaseNG.ISnapshot
            , IDatabaseNG.ITransaction
            , IDisposable
        {
            private readonly IDatabaseBlobStore.ITransaction transaction;

            public Transaction(IDatabaseSerializer serializer,
                               IDatabaseBlobStore.ITransaction transaction)
                : base(serializer)
            {
                this.transaction = transaction;
            }

            protected override IDatabaseBlobStore.ISnapshot BlobStoreSnapshot => transaction.Snapshot;

            // Entry
            public void CreateEntry(DatabaseEntry entry)
            {
                transaction.Create(serializer.SerializeEntry(entry));
            }

            public void UpdateEntry(DatabaseEntry entry)
            {
                transaction.Update(serializer.SerializeEntry(entry));
            }

            public bool DeleteEntry(EntryId id)
            {
                return transaction.Delete(BlobId.From(id));
            }

            // User
            public void CreateUser(DatabaseUser user)
            {
                transaction.Create(serializer.SerializeUser(user));
            }

            public void UpdateUser(DatabaseUser user)
            {
                transaction.Update(serializer.SerializeUser(user));
            }

            public bool DeleteUser(UserId id)
            {
                return transaction.Delete(BlobId.From(id));
            }

            // Misc
            public void Commit()
            {
                transaction.Commit();
            }

            public void Dispose()
            {
                transaction.Dispose();
            }
        }
    }
}
