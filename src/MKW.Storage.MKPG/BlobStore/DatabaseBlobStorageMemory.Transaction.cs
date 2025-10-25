// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Storage.Exceptions;

namespace MKW.Storage.MKPG.BlobStore
{
    internal sealed partial class DatabaseBlobStorageMemory
    {
        private sealed class Transaction : IDatabaseBlobStore.ITransaction
        {
            private readonly DatabaseBlobStorageMemory database;
            private readonly Dictionary<BlobId, Blob> blobs;

            public Transaction(DatabaseBlobStorageMemory database,
                               IReadOnlyCollection<Blob> blobs)
            {
                this.database = database;

                this.blobs = new Dictionary<BlobId, Blob>(blobs.Count);
                foreach (Blob blob in blobs)
                {
                    this.blobs[blob.Id] = blob;
                }
            }

            public void Create(Blob blob)
            {
                if (blobs.ContainsKey(blob.Id))
                {
                    throw new EntryAlreadyExistsException();
                }

                blobs[blob.Id] = blob;
            }

            public bool Delete(BlobId blobId)
            {
                return blobs.Remove(blobId);
            }

            public IEnumerable<Blob> Enumerate()
            {
                return blobs.Values;
            }

            public void Update(Blob blob)
            {
                blobs[blob.Id] = blob;
            }

            public void Commit()
            {
                database.blobs = [.. blobs.Values];
            }

            public void Dispose()
            {
                /* no-op */
            }
        }
    }
}
