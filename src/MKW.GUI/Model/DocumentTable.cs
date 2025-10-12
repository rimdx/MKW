using MKW.Cryptography;
using MKW.Cryptography.Loader;

namespace MKW.GUI.Model
{
    public sealed class DocumentTable
    {
        private readonly ICryptographyProvider cryptographyProvider;

        public DocumentTable()
        {
            cryptographyProvider = CryptographyLoader.GetProvider();
        }

        public IDocumentLock CreateDatabase(string databasePath, string password)
        {
             return new DocumentLock(DatabaseModel.Create(cryptographyProvider, databasePath, password));
        }

        public IDocumentLock OpenTable(string filename)
        {
            return new DocumentLock(DatabaseModel.Open(cryptographyProvider, filename));
        }

        private class DocumentLock : IDocumentLock
        {
            public DatabaseModel Database { get; }

            public DocumentLock(DatabaseModel databaseModel)
            {
                Database = databaseModel;
            }

            public void Dispose()
            {
                // TODO: 
            }
        }
    }
}
