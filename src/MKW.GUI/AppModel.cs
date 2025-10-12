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

        public DatabaseModel CreateDatabase(string databasePath, string password)
        {
            return documentTable.CreateDatabase(databasePath, password).Database;
        }

        public DatabaseModel OpenDatabase(string filename)
        {
            return documentTable.OpenTable(filename).Database;
        }
    }
}
