using MKW.Core.Client.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public interface IEntryController : IDisposable
    {
        IEntrySession CreateEntry();
        IEntrySession CreateEntry(EntryId id);

        EntryInfo DeleteEntry(EntryId id);

        IEntrySession OpenEntry(EntryId id);

        EntryInfo UpdateEntry(EntryId id, EntryPayload? payload);

        IEnumerable<IEntrySession> EnumerateEntries();
    }
}
