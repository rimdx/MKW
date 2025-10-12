using System.IO;

namespace MKW.GUI.Model
{
    public sealed class DocumentTable
    {
        private readonly List<Document> documents;

        public DocumentTable()
        {
            documents = [];
        }

        public IDocumentLock OpenDocument(string filename, Func<DatabaseModel> documentDataFactory)
        {
            Document? doc = GetDocumentByPath(filename);
            if (doc == null)
            {
                doc = AddDocument(documentDataFactory());
            }

            return doc.ObtainLock();
        }

        private Document AddDocument(DatabaseModel databaseModel)
        {
            Document doc = Document.Make(this, databaseModel);

            documents.Add(doc);

            return doc;
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

        private sealed class Document : IDisposable
        {
            public DatabaseModel Database { get; }
            private int lockCount;
            private bool disposed;
            private readonly DocumentTable documentTable;

            private Document(DocumentTable documentTable, DatabaseModel database)
            {
                this.documentTable = documentTable;
                Database = database;
                lockCount = 0;
            }

            public static Document Make(DocumentTable documentTable, DatabaseModel database)
            {
                return new Document(documentTable, database);
            }

            public IDocumentLock ObtainLock()
            {
                lockCount++;

                return new DocumentLock(this);
            }

            public void Dispose()
            {
                if (!disposed)
                {
                    Database.Dispose();
                    documentTable.documents.Remove(this);

                    disposed = true;
                }
            }

            private void ReleaseLock()
            {
                lockCount--;
                if (lockCount == 0)
                {
                    Dispose();
                }
            }

            private sealed class DocumentLock : IDocumentLock
            {
                private readonly Document document;
                private bool disposed;

                public DocumentLock(Document document)
                {
                    this.document = document;
                }

                public DatabaseModel Database => document.Database;

                public IDocumentLock Clone()
                {
                    return document.ObtainLock();
                }

                public void Dispose()
                {
                    if (!disposed)
                    {
                        document.ReleaseLock();
                        disposed = true;
                    }
                }
            }
        }
    }
}
