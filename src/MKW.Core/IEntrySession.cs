using MKW.Core.Storage;

namespace MKW.Core
{
    public interface IEntrySession : IDisposable
    {
        EntryId Id { get; }

        EntryInfo UpdatePayload(EntryPayload payload);
        EntryPayload? OpenPayload();

        void AddAccess(UserId userId);
        IEnumerable<UserInfo> EnumerateAccess();
    }
}
