// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Collections.Immutable;

namespace MKW.Storage.MKPG.BlobStore
{
    public sealed partial class DatabaseBlobStorageMemory : IDatabaseBlobStore
    {
        private ImmutableDictionary<BlobId, Blob> blobs;

        public DatabaseBlobStorageMemory()
        {
            blobs = ImmutableDictionary<BlobId, Blob>.Empty;
        }

        public IDatabaseBlobStore.ITransaction BeginTransaction()
        {
            return new Transaction(this, blobs.Values);
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
