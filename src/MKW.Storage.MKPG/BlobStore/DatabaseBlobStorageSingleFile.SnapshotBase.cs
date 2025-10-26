// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Storage.MKPG.PgpBlob;

namespace MKW.Storage.MKPG.BlobStore
{
    internal sealed partial class DatabaseBlobStorageSingleFile
    {
        private abstract class SnapshotBase : IDatabaseBlobStore.ISnapshot
        {
            protected abstract IEnumerable<PgpBlobEntry> Blobs { get; }

            public IEnumerable<Blob> Enumerate()
            {
                foreach (PgpBlobEntry blob in Blobs)
                {
                    yield return blob.GetTypedBlob();
                }
            }
        }
    }
}
