// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Collections.Immutable;

namespace MKW.Storage.MKPG.BlobStore
{
    internal sealed partial class DatabaseBlobStorageMemory
    {
        private sealed class Snapshot : IDatabaseBlobStore.ISnapshot
        {
            private readonly ImmutableDictionary<BlobId, Blob> blobs;

            public Snapshot(ImmutableDictionary<BlobId, Blob> blobs)
            {
                this.blobs = blobs;
            }

            public IEnumerable<Blob> Enumerate()
            {
                return blobs.Values;
            }
        }
    }
}
