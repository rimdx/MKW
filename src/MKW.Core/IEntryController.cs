namespace MKW.Core
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
