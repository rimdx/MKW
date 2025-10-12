namespace MKW.Core
{
    public interface IUserSession : IDisposable
    {
        UserId Id { get; }

        IEntrySession CreateEntry();
        IEntrySession CreateEntry(EntryId id);

        void DeleteEntry(EntryId id);

        IEntrySession OpenEntry(EntryId id);

        IEnumerable<IEntrySession> EnumerateEntries();

        IEnumerable<UserId> EnumerateTrustedUsers();

        bool VerifyTrust(UserId userId);

        UserMetadata OpenMetadata();
    }
}
