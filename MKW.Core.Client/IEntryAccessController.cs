using MKW.Core.Client.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public interface IEntryAccessController : IDisposable
    {
        IEnumerable<UserInfo> EnumerateEncodedForUsers();
        EntryInfo Share(UserId userId);
    }
}
