using MKW.Core.Serialization.KeePassXmlV2;
using System.IO;

namespace MKW.GUI.Model
{
    public sealed class KeePassXmlV2BackupFormat : IBackupFormat
    {
        public string Name => "KeePass XML (2.x)";

        public IBackup Open(Stream file)
        {
            return new KeePassXmlV2Backup(new KeePassXmlV2Reader(file));
        }
    }
}
