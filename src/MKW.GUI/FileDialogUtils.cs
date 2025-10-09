using Microsoft.Win32;

namespace MKW.GUI
{
    public static class FileDialogUtils
    {
        public static FileDialog CreateOpenDatabaseDialog()
        {
            return new OpenFileDialog
            {
                DefaultExt = ".mkw",
                Filter = "Multi-Key Wallet Database File|*.mkw"
            };
        }

        public static FileDialog CreateSaveDatabaseDialog()
        {
            return new SaveFileDialog
            {
                FileName = "New Database",
                DefaultExt = ".mkw",
                Filter = "Multi-Key Wallet Database File|*.mkw"
            };
        }

        public static OpenFolderDialog CreateSelectFolderDialog(string folderName)
        {
            return new OpenFolderDialog
            {
                InputPath = folderName,
            };
        }

        public static FileDialog CreateOpenBackupDialog()
        {
            return new OpenFileDialog
            {
                DefaultExt = ".xml",
                Filter = "KeePass XML (2.x)|*.xml"
            };
        }
    }
}
