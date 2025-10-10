using System.IO;

namespace MKW.GUI.Model
{
    public sealed class KeePassXmlV1BackupFormat : IBackupFormat
    {
        public string Name => "KeePass XML (1.x)";

        public IBackup Open(Stream file)
        {
            return new KeePassXmlV1Backup(file);
        }
    }
}
