namespace MKW.Core
{
    public interface IEntrySession : IDisposable
    {
        EntryId Id { get; }

        EntryInfo UpdatePayload(EntryPayload payload);
        EntryPayload? OpenPayload();

        IEnumerable<UserId> EnumerateAccess();
    }
}
