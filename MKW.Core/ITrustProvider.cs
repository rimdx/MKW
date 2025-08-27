using MKW.Core.Client.Notify;

namespace MKW.Core.Client
{
    public interface ITrustProvider : IDisposable
    {
        IEnumerable<UserInfo> EnumerateUsersTrust();
        IEnumerable<UserInfo> EnumerateImplicitlyTrustedUsers();
        IEnumerable<UserInfo> EnumerateExplicitlyTrustedUsersInfo();
    }
}
