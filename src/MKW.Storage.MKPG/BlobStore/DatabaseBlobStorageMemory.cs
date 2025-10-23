// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.MKPG.BlobStore
{
    internal sealed partial class DatabaseBlobStorageMemory : IDatabaseBlobStore
    {
        private IReadOnlyList<Blob> blobs;

        public DatabaseBlobStorageMemory()
        {
            blobs = [];
        }

        public IDatabaseBlobStore.ITransaction BeginTransaction()
        {
            return new Transaction(this, blobs);
        }

        public IDatabaseBlobStore.ISnapshot CreateSnapshot()
        {
            return new Snapshot(blobs);
        }

        public void Dispose()
        {
        }
    }
}
