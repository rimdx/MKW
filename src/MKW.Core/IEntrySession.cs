namespace MKW.Core
{
    public interface IEntrySession : IDisposable
    {
        EntryId Id { get; }

        EntryInfo UpdatePayload(EntryPayload payload);
        EntryPayload? OpenPayload();

        void UpdateKey();
        IEnumerable<UserId> EnumerateAccess();
    }
}
