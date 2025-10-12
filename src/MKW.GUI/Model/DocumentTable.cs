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

        public DatabaseModel CreateDatabase(string databasePath, string password)
        {
             return DatabaseModel.Create(cryptographyProvider, databasePath, password);
        }

        public DatabaseModel OpenTable(string filename)
        {
            return DatabaseModel.Open(cryptographyProvider, filename);
        }
    }
}
