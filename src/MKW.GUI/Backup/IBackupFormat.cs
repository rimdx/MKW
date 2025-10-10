using System.IO;

namespace MKW.GUI.Backup
{
    public interface IBackupFormat
    {
        string Name { get; }

        IBackup Open(Stream file);
    }
}
