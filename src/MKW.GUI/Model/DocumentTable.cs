using System.IO;

namespace MKW.GUI.Model
{
    public sealed partial class DocumentTable
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
    }
}
