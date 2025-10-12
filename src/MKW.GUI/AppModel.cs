using MKW.GUI.Model;

namespace MKW.GUI
{
    public class AppModel
    {
        private readonly DocumentTable documentTable;

        public AppModel()
        {
            documentTable = new DocumentTable();
        }

        public IDocumentLock CreateDatabase(string databasePath, string password)
        {
            return documentTable.CreateDatabase(databasePath, password);
        }

        public IDocumentLock OpenDatabase(string filename)
        {
            return documentTable.OpenDatabase(filename);
        }
    }
}
