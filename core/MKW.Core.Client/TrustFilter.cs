using MKW.Core.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class TrustFilter : ITrustProvider, IDisposable
    {
        protected readonly ITrustProvider proxy;

        public TrustFilter(ITrustProvider proxy)
        {
            this.proxy = proxy;
        }

        protected virtual bool IsTrusted(UserInfo user) => user.Trust switch
        {
            Trust.None => false,
            Trust.Unknown => false,
            Trust.ExplicitTrust => true,
            Trust.ImplicitTrust => true,
            Trust.SelfTrust => true,
        };

        public IEnumerable<UserInfo> EnumerateExplicitlyTrustedUsers()
        {
            foreach (UserInfo user in proxy.EnumerateExplicitlyTrustedUsers())
            {
                if (IsTrusted(user))
                {
                    yield return user;
                }
            }
        }

        public IEnumerable<UserInfo> EnumerateImplicitlyTrustedUsers()
        {
            foreach (UserInfo user in proxy.EnumerateImplicitlyTrustedUsers())
            {
                if (IsTrusted(user))
                {
                    yield return user;
                }
            }
        }

        public Trust GetExplicitTrust(ReadOnlySpan<byte> publicKey)
        {
            throw new NotSupportedException();
        }

        public Trust GetImplicitTrust(UserId userId)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            proxy.Dispose();
        }
    }
}
