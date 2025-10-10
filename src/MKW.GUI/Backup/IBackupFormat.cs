using System.IO;

namespace MKW.GUI.Backup
{
    public interface IBackupFormat
    {
        string Name { get; }
        IReadOnlyList<string> FileExtensions { get; }

        IBackupReader Open(Stream file);
    }
}
