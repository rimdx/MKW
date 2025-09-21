using MKW.Core.Notify;
using MKW.Core.Storage;

namespace MKW.Core
{
    public interface ITrustProvider : IDisposable
    {
        IEnumerable<UserInfo> EnumerateImplicitlyTrustedUsers();
 
        Trust GetImplicitTrust(UserId userId);
    }
}
