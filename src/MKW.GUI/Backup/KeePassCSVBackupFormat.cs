using System.IO;

namespace MKW.GUI.Backup
{
    public sealed class KeePassCSVBackupFormat : IBackupFormat
    {
        public string Name => "KeePass CSV (1.x)";

        public IReadOnlyList<string> FileExtensions => ["*.csv"];

        public IBackupReader OpenRead(Stream file)
        {
            return new KeePassCSVBackup(file);
        }

        public IBackupWriter OpenWrite(Stream file)
        {
            throw new NotSupportedException();
        }
    }
}
