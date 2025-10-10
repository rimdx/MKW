using MKW.Core;

namespace MKW.GUI.Backup
{
    public interface IBackup : IDisposable
    {
        IEnumerable<EntryPayload> EnumerateEntries();
    }
}
