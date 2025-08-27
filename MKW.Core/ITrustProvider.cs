using MKW.Core.Client.Notify;

namespace MKW.Core.Client
{
    public interface ITrustProvider : IDisposable
    {
        IEnumerable<UserInfo> EnumerateImplicitlyTrustedUsers();
        IEnumerable<UserInfo> EnumerateExplicitlyTrustedUsers();

        Trust VerifyTrust(ReadOnlySpan<byte> publicKey);
    }
}
