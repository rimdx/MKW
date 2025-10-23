// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

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
                    if (blob.Type == MKPGConstants.ArmourTypeHeaders.User)
                    {
                        yield return new BlobUser
                        {
                            Id = blob.Id,
                            Data = blob.Data,
                        };
                    }
                    else if (blob.Type == MKPGConstants.ArmourTypeHeaders.Entry)
                    {
                        yield return new BlobSecretEntry
                        {
                            Id = blob.Id,
                            Data = blob.Data,
                        };
                    }
                    else
                    {
                        throw new Exception($"Unknown blob type: {blob.Type}");
                    }
                }
            }

            public abstract void Dispose();
        }
    }
}
