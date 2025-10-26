// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.MKPG.BlobStore
{
    internal sealed partial class DatabaseBlobStorageMemory
    {
        private sealed class Snapshot : IDatabaseBlobStore.ISnapshot
        {
            private readonly IReadOnlyCollection<Blob> blobs;

            public Snapshot(IReadOnlyCollection<Blob> blobs)
            {
                this.blobs = [.. blobs];
            }

            public IEnumerable<Blob> Enumerate()
            {
                return blobs;
            }
        }
    }
}
