// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Storage.MKPG.PgpBlob;

namespace MKW.Storage.MKPG.BlobStore
{
    public sealed partial class DatabaseBlobStorageSingleFile
    {
        private sealed class Snapshot : SnapshotBase
        {
            protected override IEnumerable<PgpBlobEntry> Blobs { get; }

            public Snapshot(IEnumerable<PgpBlobEntry> blobs)
            {
                Blobs = [.. blobs];
            }
        }
    }
}
