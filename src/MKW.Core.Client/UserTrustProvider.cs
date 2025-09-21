using MKW.Core.Notify;
using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    public class UserTrustProvider : ITrustProvider, IDisposable
    {
        public UserId UserId => admin.Id;

        protected readonly IDatabase database;
        protected readonly ICryptographyProvider crypto;
        protected readonly IDatabaseUser admin;
        protected readonly ITrustVerifier trustVerifier;

        public UserTrustProvider(IDatabase database,
                                 ICryptographyProvider crypto,
                                 IDatabaseUser admin)
        {
            this.database = database;
            this.crypto = crypto;
            this.admin = admin;
            trustVerifier = new UserTrustVerifier(crypto, admin);
        }

        public IEnumerable<UserInfo> EnumerateImplicitlyTrustedUsers()
        {
            foreach (IDatabaseUser user in database.EnumerateUsers())
            {
                Trust trust = trustVerifier.GetTrust(user.PublicKey.Span);

                if (trust == Trust.ExplicitTrust || trust == Trust.SelfTrust)
                {
                    yield return UserInfo.FromDatabaseUser(user, trust);
                }
            }
        }

        public Trust GetImplicitTrust(UserId userId)
        {
            IDatabaseUser user = database.OpenUser(userId, true);
            return trustVerifier.GetTrust(user.PublicKey.Span);
        }

        public virtual void Dispose()
        {
            trustVerifier.Dispose();
        }
    }
}
