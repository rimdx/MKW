using MKW.Core.Notify;
using MKW.Core.Storage;

namespace MKW.Core
{
    public interface IEntrySession : IEntryAccessController, IDisposable
    {
        EntryId Id { get; }

        EntryInfo UpdatePayload(EntryPayload payload);
        EntryPayload? OpenPayload();
    }
}
