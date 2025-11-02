// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Storage.Exceptions;
using System.Collections.Immutable;

namespace MKW.Storage.MKPG.BlobStore
{
    public sealed partial class DatabaseBlobStorageMemory
    {
        private sealed class Transaction : IDatabaseBlobStore.ITransaction
        {
            private readonly DatabaseBlobStorageMemory database;
            private readonly ImmutableDictionary<BlobId, Blob>.Builder blobs;

            public Transaction(DatabaseBlobStorageMemory database,
                               IEnumerable<Blob> blobs)
            {
                this.database = database;

                this.blobs = ImmutableDictionary.CreateBuilder<BlobId, Blob>();
                foreach (Blob blob in blobs)
                {
                    this.blobs[blob.Id] = blob;
                }
            }

            public IDatabaseBlobStore.ISnapshot Snapshot
            {
                get
                {
                    return new Snapshot(blobs.ToImmutable());
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
                database.blobs = blobs.ToImmutable();
            }

            public void Dispose()
            {
                /* no-op */
            }
        }
    }
}
