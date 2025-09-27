using MKW.GUI.Model;

namespace MKW.GUI
{
    public class AppModel
    {
        public DatabaseModel? CreateDatabase(string databasePath, string password)
        {
            return DatabaseModel.Create(databasePath, password);
        }

        public DatabaseModel OpenDatabase(string filename)
        {
            return DatabaseModel.Open(filename);
        }
    }
}
