using System.IO;

namespace MKW.GUI.Backup
{
    public sealed class KeePassXmlV1BackupFormat : IBackupFormat
    {
        public string Name => "KeePass XML (1.x)";

        public IReadOnlyList<string> FileExtensions => ["*.xml"];

        public IBackupReader OpenRead(Stream file)
        {
            return new KeePassXmlV1Backup(file);
        }

        public IBackupWriter OpenWrite(Stream file)
        {
            throw new NotSupportedException();
        }
    }
}
