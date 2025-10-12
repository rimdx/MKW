using MKW.Cryptography;
using MKW.Cryptography.Loader;
using System.IO;

namespace MKW.GUI.Model
{
    public sealed class DocumentTable
    {
        private readonly ICryptographyProvider cryptographyProvider;
        private readonly List<Document> documents;

        public DocumentTable()
        {
            cryptographyProvider = CryptographyLoader.GetProvider();
            documents = [];
        }

        public IDocumentLock CreateDatabase(string databasePath, string password)
        {
            Document? doc = GetDocumentByPath(databasePath);
            if (doc == null)
            {
                doc = new Document(DatabaseModel.Create(cryptographyProvider, databasePath, password));
            }

            return new DocumentLock(doc);
        }

        public IDocumentLock OpenTable(string filename)
        {
            Document? doc = GetDocumentByPath(filename);
            if (doc == null)
            {
                doc = new Document(DatabaseModel.Open(cryptographyProvider, filename));
            }

            return new DocumentLock(doc);
        }

        private Document? GetDocumentByPath(string path)
        {
            string fullPath = Path.GetFullPath(path);

            foreach (Document doc in documents)
            {
                if (string.Compare(Path.GetFullPath(doc.Database.Path), fullPath, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return doc;
                }
            }

            return null;
        }

        private class Document : IDisposable
        {
            public DatabaseModel Database { get; }

            public Document(DatabaseModel database)
            {
                Database = database;
            }

            public void Dispose()
            {
                Database.Dispose();
            }
        }

        private class DocumentLock : IDocumentLock
        {
            private readonly Document document;

            public DocumentLock(Document document)
            {
                this.document = document;
            }

            public DatabaseModel Database => document.Database;

            public void Dispose()
            {
                // TODO: 
            }
        }
    }
}
