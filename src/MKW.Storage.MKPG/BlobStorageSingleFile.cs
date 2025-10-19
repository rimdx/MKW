// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Storage.Exceptions;

namespace MKW.Storage.MKPG
{
    internal sealed class BlobStorageSingleFile : IBlobStorage
    {
        private readonly IFileEditorFactory editor;

        public BlobStorageSingleFile(IFileEditorFactory editor)
        {
            this.editor = editor;
        }

        public void Create(BlobEntry entry)
        {
            using IFileEditorTransaction transaction = editor.CreateTransaction();

            CreateInternal(transaction, entry);
            transaction.Commit();
        }

        private static void CreateInternal(IFileEditorTransaction transaction, BlobEntry entry)
        {
            using StreamReader reader = new StreamReader(transaction.Reader);
            using StreamWriter writer = new StreamWriter(transaction.Writer);

            foreach (BlobEntry blob in BlobStorageSerializer.ReadBlobs(reader))
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

        public void Update(BlobEntry entry)
        {
            using IFileEditorTransaction transaction = editor.CreateTransaction();

            UpdateInternal(transaction, entry);
            transaction.Commit();
        }

        private static void UpdateInternal(IFileEditorTransaction transaction, BlobEntry entry)
        {
            using StreamReader reader = new StreamReader(transaction.Reader);
            using StreamWriter writer = new StreamWriter(new StreamDisown(transaction.Writer));

            int updated = 0;
            foreach (BlobEntry blob in BlobStorageSerializer.ReadBlobs(reader))
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

        public BlobEntry Open(BlobId id)
        {
            using StreamReader reader = new StreamReader(editor.CreateReader());

            foreach (BlobEntry blob in BlobStorageSerializer.ReadBlobs(reader))
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
            throw new NotImplementedException();
        }

        public bool Exists(BlobId id)
        {
            using StreamReader reader = new StreamReader(editor.CreateReader());

            foreach (BlobEntry blob in BlobStorageSerializer.ReadBlobs(reader))
            {
                if (blob.Id.Equals(id))
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<BlobEntry> Enumerate()
        {
            using StreamReader reader = new StreamReader(editor.CreateReader());

            foreach (BlobEntry blob in BlobStorageSerializer.ReadBlobs(reader))
            {
                yield return blob;
            }
        }
    }
}
