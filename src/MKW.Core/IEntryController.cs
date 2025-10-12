namespace MKW.Core
{
    public interface IEntryController : IDisposable
    {
        IEntrySession CreateEntry();
        IEntrySession CreateEntry(EntryId id);

        EntryInfo DeleteEntry(EntryId id);

        IEntrySession OpenEntry(EntryId id);

        IEnumerable<IEntrySession> EnumerateEntries();
    }
}
