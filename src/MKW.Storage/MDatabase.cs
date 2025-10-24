// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage
{
    public sealed partial class MDatabase
        : IDatabase3
    {
        private readonly IDatabaseSerializer serializer;
        private readonly IDatabaseBlobStore store;

        public MDatabase(IDatabaseSerializer serializer, IDatabaseBlobStore store)
        {
            this.serializer = serializer;
            this.store = store;
        }

        public IDatabase3.ITransaction BeginTransaction()
        {
            return new Transaction(serializer, store.BeginTransaction());
        }

        public IDatabase3.ISnapshot CreateSnapshot()
        {
            return new Snapshot(serializer, store.CreateSnapshot());
        }

        public DatabaseConfiguration GetConfiguration()
        {
            throw new NotImplementedException();
        }

        public void ReloadDatabaseFile()
        {
            throw new NotImplementedException();
        }

        public Task<bool> WaitForDatabaseChangesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
