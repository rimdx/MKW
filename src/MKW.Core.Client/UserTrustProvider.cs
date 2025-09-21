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

        public Trust GetTrust(ReadOnlySpan<byte> otherPublicKey)
        {
            if (otherPublicKey.SequenceEqual(admin.PublicKey.Span))
            {
                return Trust.SelfTrust;
            }

            using IAsymmetricPublicTransformer publicKey = crypto.OpenAsymmetricTransformer(admin.PublicKey.Span);

            foreach (ReadOnlyMemory<byte> trust in admin.EnumerateTrust())
            {
                if (publicKey.Verify(otherPublicKey, trust.Span))
                {
                    return Trust.ExplicitTrust;
                }
            }

            return Trust.None;
        }

        public IEnumerable<UserInfo> EnumerateImplicitlyTrustedUsers()
        {
            foreach (IDatabaseUser user in database.EnumerateUsers())
            {
                Trust trust = GetTrust(user.PublicKey.Span);

                if (trust == Trust.ExplicitTrust || trust == Trust.SelfTrust)
                {
                    yield return UserInfo.FromDatabaseUser(user, trust);
                }
            }
        }

        public Trust GetImplicitTrust(UserId userId)
        {
            IDatabaseUser user = database.OpenUser(userId, true);
            return GetTrust(user.PublicKey.Span);
        }

        public virtual void Dispose()
        {
        }
    }
}
