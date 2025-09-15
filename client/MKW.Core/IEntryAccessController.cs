using MKW.Core.Notify;
using MKW.Core.Storage;

namespace MKW.Core
{
    public interface IEntryAccessController : IDisposable
    {
        IEnumerable<UserInfo> EnumerateAccess();
        void AddAccess(UserId userId);
    }
}
