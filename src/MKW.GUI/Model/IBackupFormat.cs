using System.IO;

namespace MKW.GUI.Model
{
    public interface IBackupFormat
    {
        string Name { get; }

        IBackup Open(Stream file);
    }
}
