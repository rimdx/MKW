using System.IO;

namespace MKW.GUI.Backup
{
    public sealed class KeePassXmlV2BackupFormat : IBackupFormat
    {
        public string Name => "KeePass XML (2.x)";

        public IReadOnlyList<string> FileExtensions => ["*.xml"];

        public IBackup Open(Stream file)
        {
            return new KeePassXmlV2Backup(file);
        }
    }
}
