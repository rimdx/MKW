using MKW.Core;

namespace MKW.GUI.Model
{
    public interface IBackup
    {
        IEnumerable<EntryPayload> EnumerateEntries();
    }
}
