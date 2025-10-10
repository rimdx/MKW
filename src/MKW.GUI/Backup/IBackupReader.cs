using MKW.Core;

namespace MKW.GUI.Backup
{
    public interface IBackupReader : IDisposable
    {
        IEnumerable<EntryPayload> EnumerateEntries();
    }
}
