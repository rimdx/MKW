using MKW.Core.Client.Notify;

namespace MKW.Core.Client
{
    /// <summary>
    /// Used to evaluate trust network.
    ///
    /// Each user in a trust network may have references to other users which they
    /// explicitly trust (use <c>EnumerateExplicitlyTrustedUsers()</c> function to
    /// retrieve such or <c>VerifyTrust()</c> to check whether one's public key is
    /// trust or not).
    ///
    /// Based on explicit trusts of each user, a complete trust network can be
    /// built.  Trust providers should implement a recursive algorithm that walks
    /// through explicitly trusted users, and then their children, etc.  Use the
    /// <c>EnumerateImplicitlyTrustedUsers()</c> method for such.
    ///
    /// In most of the cases, if no custom behaviour is needed, user should rely
    /// on the <c>EnumerateImplicitlyTrustedUsers()</c> method in order to retrieve
    /// all the users they trust, i.e. when encoding entries.
    /// </summary>
    public interface ITrustProvider : IDisposable
    {
        IEnumerable<UserInfo> EnumerateImplicitlyTrustedUsers();
        IEnumerable<UserInfo> EnumerateExplicitlyTrustedUsers();

        Trust GetExplicitTrust(ReadOnlySpan<byte> publicKey);
    }
}
