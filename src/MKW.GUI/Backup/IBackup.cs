using MKW.Core;

namespace MKW.GUI.Backup
{
    public interface IBackup
    {
        IEnumerable<EntryPayload> EnumerateEntries();
    }
}
