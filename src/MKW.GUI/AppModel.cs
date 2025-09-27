using MKW.Cryptography;
using MKW.Cryptography.Loader;
using MKW.GUI.Model;

namespace MKW.GUI
{
    public class AppModel
    {
        private readonly ICryptographyProvider cryptographyProvider;

        public AppModel()
        {
            cryptographyProvider = CryptographyLoader.GetProvider();
        }

        public DatabaseModel CreateDatabase(string databasePath, string password)
        {
            return DatabaseModel.Create(cryptographyProvider, databasePath, password);
        }

        public DatabaseModel OpenDatabase(string filename)
        {
            return DatabaseModel.Open(cryptographyProvider, filename);
        }
    }
}
