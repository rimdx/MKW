// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Storage.Exceptions;
using MKW.Storage.MKPG.FileSystem;
using MKW.Storage.MKPG.PgpBlob;

namespace MKW.Storage.MKPG.BlobStore
{
    internal sealed class BlobStorageSingleFile : IBlobStorage
    {
        private readonly IFileEditorFactory editor;

        public BlobStorageSingleFile(IFileEditorFactory editor)
        {
            this.editor = editor;
        }

        public void Create(PgpBlobEntry entry)
        {
            using IFileEditorFactory.ITransaction transaction = editor.CreateTransaction();

            CreateInternal(transaction, entry);
            transaction.Commit();
        }

        private static void CreateInternal(IFileEditorFactory.ITransaction transaction, PgpBlobEntry entry)
        {
            using StreamReader reader = new StreamReader(transaction.Reader);
            using StreamWriter writer = new StreamWriter(transaction.Writer);

            foreach (PgpBlobEntry blob in BlobStorageSerializer.ReadBlobs(reader))
            {
                if (blob.Id.Equals(entry.Id))
                {
                    throw new EntryAlreadyExistsException();
                }
                else
                {
                    BlobStorageSerializer.WriteBlob(writer, blob);
                }
            }

            BlobStorageSerializer.WriteBlob(writer, entry);
        }

        public void Update(PgpBlobEntry entry)
        {
            using IFileEditorFactory.ITransaction transaction = editor.CreateTransaction();

            UpdateInternal(transaction, entry);
            transaction.Commit();
        }

        private static void UpdateInternal(IFileEditorFactory.ITransaction transaction, PgpBlobEntry entry)
        {
            using StreamReader reader = new StreamReader(transaction.Reader);
            using StreamWriter writer = new StreamWriter(new StreamDisown(transaction.Writer));

            int updated = 0;
            foreach (PgpBlobEntry blob in BlobStorageSerializer.ReadBlobs(reader))
            {
                if (blob.Id.Equals(entry.Id))
                {
                    BlobStorageSerializer.WriteBlob(writer, entry);
                    updated++;
                }
                else
                {
                    BlobStorageSerializer.WriteBlob(writer, blob);
                }
            }

            if (updated == 0)
            {
                throw new EntryDoesNotExistException();
            }

            if (updated > 1)
            {
                throw new DatabaseCorruptedException();
            }
        }

        public PgpBlobEntry Open(BlobId id)
        {
            using StreamReader reader = new StreamReader(editor.CreateReader());

            foreach (PgpBlobEntry blob in BlobStorageSerializer.ReadBlobs(reader))
            {
                if (blob.Id.Equals(id))
                {
                    return blob;
                }
            }

            throw new EntryAlreadyExistsException();
        }

        public bool Delete(BlobId id)
        {
            using IFileEditorFactory.ITransaction transaction = editor.CreateTransaction();

            bool result = DeleteInternal(transaction, id);
            transaction.Commit();

            return result;
        }

        private static bool DeleteInternal(IFileEditorFactory.ITransaction transaction, BlobId id)
        {
            using StreamReader reader = new StreamReader(transaction.Reader);
            using StreamWriter writer = new StreamWriter(new StreamDisown(transaction.Writer));

            int updated = 0;
            foreach (PgpBlobEntry blob in BlobStorageSerializer.ReadBlobs(reader))
            {
                if (blob.Id.Equals(id))
                {
                    updated++;
                }
                else
                {
                    BlobStorageSerializer.WriteBlob(writer, blob);
                }
            }

            if (updated == 0)
            {
                return false;
            }
            else if (updated == 1)
            {
                return true;
            }
            else
            {
                throw new DatabaseCorruptedException();
            }
        }

        public bool Exists(BlobId id)
        {
            using StreamReader reader = new StreamReader(editor.CreateReader());

            foreach (PgpBlobEntry blob in BlobStorageSerializer.ReadBlobs(reader))
            {
                if (blob.Id.Equals(id))
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<PgpBlobEntry> Enumerate()
        {
            using StreamReader reader = new StreamReader(editor.CreateReader());

            foreach (PgpBlobEntry blob in BlobStorageSerializer.ReadBlobs(reader))
            {
                yield return blob;
            }
        }
    }
}
