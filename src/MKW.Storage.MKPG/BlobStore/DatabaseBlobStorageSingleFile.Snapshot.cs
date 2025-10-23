// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.MKPG.BlobStore
{
    internal sealed partial class DatabaseBlobStorageSingleFile
    {
        private sealed class Snapshot : SnapshotBase
        {
            protected override IEnumerable<PgpBlobEntry> Blobs { get; }

            public Snapshot(Stream steam)
            {
                using StreamReader reader = new StreamReader(steam);
                Blobs = [.. BlobStorageSerializer.ReadBlobs(reader)];
            }

            public override void Dispose()
            {
            }
        }
    }
}
