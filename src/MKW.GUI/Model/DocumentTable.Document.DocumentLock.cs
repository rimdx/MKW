namespace MKW.GUI.Model
{
    public sealed partial class DocumentTable
    {
        private sealed partial class Document
        {
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
