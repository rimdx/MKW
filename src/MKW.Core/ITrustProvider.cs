namespace MKW.Core
{
    public interface ITrustProvider : IDisposable
    {
        IEnumerable<UserInfo> EnumerateTrustedUsers();

        bool VerifyTrust(UserId userId);
    }
}
