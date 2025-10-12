namespace MKW.Core
{
    public interface IUserSession
        : ITrustProvider
        , IDisposable
    {
        UserId Id { get; }

        IEntrySession CreateEntry();
        IEntrySession CreateEntry(EntryId id);

        EntryInfo DeleteEntry(EntryId id);

        IEntrySession OpenEntry(EntryId id);

        IEnumerable<IEntrySession> EnumerateEntries();

        UserMetadata OpenMetadata();
    }
}
