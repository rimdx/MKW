using Microsoft.Win32;
using MKW.GUI.Backup;

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

        public static FileDialog CreateOpenBackupDialog(IBackupFormat backupFormat)
        {
            return new OpenFileDialog
            {
                Filter = MakeFilter(backupFormat.Name, backupFormat.FileExtensions)
            };
        }

        private static string MakeFilter(string description, IEnumerable<string> extensions)
        {
            return $"{description}|{string.Join(";", extensions)}";
        }
    }
}
