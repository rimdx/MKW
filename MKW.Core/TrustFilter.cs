using MKW.Core.Client.Notify;

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

        public IEnumerable<UserInfo> EnumerateUsersTrust()
        {
            foreach (UserInfo user in proxy.EnumerateUsersTrust())
            {
                if (IsTrusted(user))
                {
                    yield return user;
                }
            }
        }

        public IEnumerable<UserInfo> EnumerateExplicitlyTrustedUsersInfo()
        {
            foreach (UserInfo user in proxy.EnumerateExplicitlyTrustedUsersInfo())
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

        public Trust VerifyTrust(ReadOnlySpan<byte> publicKey)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            proxy.Dispose();
        }
    }
}
