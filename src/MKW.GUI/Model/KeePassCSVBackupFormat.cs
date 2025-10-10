using System.IO;

namespace MKW.GUI.Model
{
    public sealed class KeePassCSVBackupFormat : IBackupFormat
    {
        public string Name => "KeePass CSV (1.x)";

        public IBackup Open(Stream file)
        {
            return new KeePassCSVBackup(file);
        }
    }
}
