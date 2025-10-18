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

            if (blobs.FirstOrDefault(blob => blob.Id == entry.Id) != null)
            {
                throw new Exception("Entry already exists.");
            }

            blobs.Add(entry);

            file.Seek(0, SeekOrigin.Begin);
            using StreamWriter writer = new StreamWriter(new StreamDisown(file));

            BlobStorageSerializer.WriteBlobs(writer, blobs);
        }

        public BlobEntry Open(BlobId id)
        {
            file.Seek(0, SeekOrigin.Begin);
            using StreamReader reader = new StreamReader(new StreamDisown(file));

            foreach (BlobEntry blob in BlobStorageSerializer.ReadBlobs(reader))
            {
                if (blob.Id == id)
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
