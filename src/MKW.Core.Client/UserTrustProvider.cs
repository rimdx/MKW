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

        public UserTrustProvider(IDatabase database,
                                 ICryptographyProvider crypto,
                                 IDatabaseUser admin)
        {
            this.database = database;
            this.crypto = crypto;
            this.admin = admin;
        }

        public Trust GetTrust(IDatabaseUser user)
        {
            if (user.PublicKey.Span.SequenceEqual(admin.PublicKey.Span))
            {
                return Trust.SelfTrust;
            }

            using IAsymmetricPublicTransformer publicKey = crypto.OpenAsymmetricTransformer(admin.PublicKey.Span);

            if (publicKey.Verify(user.PublicKey.Span, user.AdminSignature.Span))
            {
                return Trust.ExplicitTrust;
            }

            return Trust.None;
        }

        public IEnumerable<UserInfo> EnumerateImplicitlyTrustedUsers()
        {
            foreach (IDatabaseUser user in database.EnumerateUsers())
            {
                Trust trust = GetTrust(user);

                if (trust == Trust.ExplicitTrust || trust == Trust.SelfTrust)
                {
                    yield return UserInfo.FromDatabaseUser(user, trust);
                }
            }
        }

        public Trust GetImplicitTrust(UserId userId)
        {
            IDatabaseUser user = database.OpenUser(userId, true);
            return GetTrust(user);
        }

        public virtual void Dispose()
        {
        }
    }
}
