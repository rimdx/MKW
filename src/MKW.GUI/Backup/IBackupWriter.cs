using MKW.Core;

namespace MKW.GUI.Backup
{
    public interface IBackupWriter : IDisposable
    {
        void WriteEntry(EntryPayload payload);
    }
}
