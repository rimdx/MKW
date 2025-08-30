using MKW.Core.Client.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public interface IEntrySession : IDisposable
    {
        EntryId Id { get; }

        EntryInfo UpdatePayload(EntryPayload payload);
        EntryPayload? OpenPayload();

        IEnumerable<UserInfo> EnumerateEncodedForUsers();
    }
}
