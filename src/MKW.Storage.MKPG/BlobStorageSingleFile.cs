using MKW.Common;
using Org.BouncyCastle.Bcpg;

namespace MKW.Storage.MKPG
{
    internal sealed class BlobStorageSingleFile : IBlobStorage
    {
        private readonly Stream file;

        public BlobStorageSingleFile(Stream file)
        {
            this.file = file;
        }

        private BlobEntry? Find(BlobId id)
        {
            file.Seek(0, SeekOrigin.Begin);

            while (true)
            {
                using ArmoredInputStream armour = new ArmoredInputStream(new StreamDisown(file), true);
                using MemoryStream buffer = new MemoryStream();

                string[] headers = armour.GetArmorHeaders();

                armour.CopyTo(buffer);

                if (buffer.Length <= 0)
                {
                    return null;
                }

                Guid currentId = new Guid(GetId(headers));

                if (id.GetGuid() == currentId)
                {
                    return new BlobEntry(buffer.ToArray());
                }
            }
        }

        private static string GetId(string[] headers)
        {
            const string prefix = "Id: ";

            foreach (string header in headers)
            {
                if (header.StartsWith(prefix))
                {
                    return header.Substring(prefix.Length);
                }
            }

            throw new Exception("Missing 'Id' header.");
        }

        public void Create(BlobId id, BlobEntry entry)
        {
            if (Find(id) == null)
            {
                file.Seek(0, SeekOrigin.End);

                using (ArmoredOutputStream armour = new ArmoredOutputStream(new StreamDisown(file)))
                {
                    armour.SetHeader("Id", id.ToString());
                    armour.Write(entry.Data.Span);
                }

                file.WriteByte((byte)'\n');
            }
            else
            {
                throw new Exception("Entry already exists.");
            }
        }

        public BlobEntry Open(BlobId id)
        {
            return Find(id) ?? throw new Exception("Entry does not already exists.");
        }

        public bool Delete(BlobId id)
        {
            throw new NotImplementedException();
        }

        public bool Exists(BlobId id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BlobId> Enumerate()
        {
            file.Seek(0, SeekOrigin.Begin);

            while (true)
            {
                using ArmoredInputStream armour = new ArmoredInputStream(new StreamDisown(file), true);
                using MemoryStream buffer = new MemoryStream();

                string[] headers = armour.GetArmorHeaders();

                armour.CopyTo(buffer);

                if (buffer.Length <= 0)
                {
                    //yield break;
                }

                Guid currentId = new Guid(GetId(headers));

                // TODO: also return content
                yield return BlobId.From(currentId);
            }
        }
    }
}
