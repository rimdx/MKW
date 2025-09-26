namespace MKW.Core
{
    public interface ITrustProvider : IDisposable
    {
        IEnumerable<UserId> EnumerateTrustedUsers();

        bool VerifyTrust(UserId userId);
    }
}
