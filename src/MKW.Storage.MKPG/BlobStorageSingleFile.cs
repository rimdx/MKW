// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;

namespace MKW.Storage.MKPG
{
    internal sealed class BlobStorageSingleFile : IBlobStorage
    {
        private readonly Stream file;

        public BlobStorageSingleFile(Stream file)
        {
            this.file = file;
        }

        public void Create(BlobEntry entry)
        {
            file.Seek(0, SeekOrigin.Begin);
            using StreamReader reader = new StreamReader(new StreamDisown(file));

            List<BlobEntry> blobs = BlobStorageSerializer.ReadBlobs(reader).ToList();

            if (blobs.FirstOrDefault(blob => blob.Id.Equals(entry.Id)) != null)
            {
                throw new Exception("Entry already exists.");
            }

            blobs.Add(entry);

            file.Seek(0, SeekOrigin.Begin);
            using StreamWriter writer = new StreamWriter(new StreamDisown(file));

            BlobStorageSerializer.WriteBlobs(writer, blobs);
        }

        public void Update(BlobEntry entry)
        {
            file.Seek(0, SeekOrigin.Begin);
            using StreamReader reader = new StreamReader(new StreamDisown(file));

            List<BlobEntry> blobs = BlobStorageSerializer.ReadBlobs(reader).ToList();
            List<BlobEntry> newBlobs = new List<BlobEntry>(blobs.Count);
            int updated = 0;

            foreach (BlobEntry blob in blobs)
            {
                if (blob.Id.Equals(entry.Id))
                {
                    newBlobs.Add(entry);
                    updated++;
                }
                else
                {
                    newBlobs.Add(blob);
                }
            }

            if (updated == 0)
            {
                throw new Exception("Entry does not exist.");
            }

            if (updated > 1)
            {
                throw new Exception("Database corrupted.");
            }

            file.Seek(0, SeekOrigin.Begin);
            using StreamWriter writer = new StreamWriter(new StreamDisown(file));

            BlobStorageSerializer.WriteBlobs(writer, newBlobs);
        }

        public BlobEntry Open(BlobId id)
        {
            file.Seek(0, SeekOrigin.Begin);
            using StreamReader reader = new StreamReader(new StreamDisown(file));

            foreach (BlobEntry blob in BlobStorageSerializer.ReadBlobs(reader))
            {
                if (blob.Id.Equals(id))
                {
                    return blob;
                }
            }

            throw new Exception("Entry already exists.");
        }

        public bool Delete(BlobId id)
        {
            throw new NotImplementedException();
        }

        public bool Exists(BlobId id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BlobEntry> Enumerate()
        {
            file.Seek(0, SeekOrigin.Begin);

            using StreamReader reader = new StreamReader(new StreamDisown(file));

            foreach (BlobEntry blob in BlobStorageSerializer.ReadBlobs(reader))
            {
                yield return blob;
            }
        }
    }
}
