// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage
{
    public sealed partial class MDatabase
    {
        private sealed class Snapshot
            : SnapshotBase
            , IDatabaseSerializer
            , IDatabase3.ISnapshot
            , IDisposable
        {
            protected override IDatabaseBlobStore.ISnapshot BlobStoreSnapshot { get; }

            public Snapshot(IDatabaseSerializer serializer,
                            IDatabaseBlobStore.ISnapshot blobStoreSnapshot)
                : base(serializer)
            {
                BlobStoreSnapshot = blobStoreSnapshot;
            }

            public override void Dispose()
            {
                BlobStoreSnapshot.Dispose();
            }
        }
    }
}
