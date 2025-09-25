using MKW.GUI.Model;

namespace MKW.GUI
{
    public class AppModel
    {
        internal DatabaseUnlockedModel? CreateDatabase(string databasePath, string password)
        {
            return DatabaseUnlockedModel.Create(databasePath, password);
        }

        internal DatabaseUnlockedModel OpenDatabase(string filename)
        {
            return DatabaseUnlockedModel.Open(filename);
        }
    }
}
