// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Storage.Exceptions;
using MKW.Storage.MKPG.FileSystem;

namespace MKW.Storage.MKPG.BlobStore
{
    internal sealed partial class DatabaseBlobStorageSingleFile
    {
        private sealed class Transaction : SnapshotBase, IDatabaseBlobStore.ITransaction
        {
            private readonly Dictionary<BlobId, PgpBlobEntry> editedEntries;
            private readonly IFileEditorFactory.ITransaction transaction;
            private bool disposed = false;

            public Transaction(IFileEditorFactory.ITransaction transaction)
            {
                using StreamReader reader = new StreamReader(transaction.Reader);

                editedEntries = [];
                foreach (PgpBlobEntry blob in BlobStorageSerializer.ReadBlobs(reader))
                {
                    editedEntries.Add(blob.Id, blob);
                }

                this.transaction = transaction;
            }

            protected override IEnumerable<PgpBlobEntry> Blobs => editedEntries.Values;

            private static string GetType(Blob blob)
            {
                return blob.Visit(new GetBlobTypeVisitor());
            }

            public void Create(Blob blob)
            {
                if (editedEntries.ContainsKey(blob.Id))
                {
                    throw new EntryAlreadyExistsException();
                }

                editedEntries[blob.Id] = new PgpBlobEntry
                {
                    Id = blob.Id,
                    Data = blob.Data,
                    Type = GetType(blob),
                };
            }

            public void Update(Blob blob)
            {
                editedEntries[blob.Id] = new PgpBlobEntry
                {
                    Id = blob.Id,
                    Data = blob.Data,
                    Type = GetType(blob),
                };
            }

            public bool Delete(BlobId blobId)
            {
                return editedEntries.Remove(blobId);
            }

            public void Commit()
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(nameof(Transaction));
                }

                using (StreamWriter writer = new StreamWriter(transaction.Writer))
                {
                    BlobStorageSerializer.WriteBlobs(writer, editedEntries.Values);
                }

                transaction.Commit();

                disposed = true;
            }

            public override void Dispose()
            {
                transaction.Dispose();
                disposed = true;
            }
        }
    }
}
