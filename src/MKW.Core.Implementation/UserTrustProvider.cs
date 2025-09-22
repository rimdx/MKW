using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Implementation
{
    public class UserTrustProvider : ITrustProvider, IDisposable
    {
        public UserId UserId => admin.Id;

        protected readonly IDatabase database;
        protected readonly ICryptographyProvider crypto;
        private readonly IAsymmetricPublicTransformer meKey;
        protected readonly IDatabaseUser admin;

        public UserTrustProvider(IDatabase database,
                                 ICryptographyProvider crypto,
                                 IAsymmetricPublicTransformer meKey,
                                 IDatabaseUser admin)
        {
            this.database = database;
            this.crypto = crypto;
            this.meKey = meKey;
            this.admin = admin;
        }

        private bool VerifyTrust(IDatabaseUser user)
        {
            // trust ourselves
            if (user.PublicKey.Span.SequenceEqual(meKey.ExportPublicKey().Span))
            {
                return true;
            }

            // trust admin
            // TODO: verify admin
            if (user.PublicKey.Span.SequenceEqual(admin.PublicKey.Span))
            {
                return true;
            }

            using IAsymmetricPublicTransformer publicKey = crypto.OpenAsymmetricTransformer(admin.PublicKey.Span);

            // otherwise verify admin trust to this user
            if (publicKey.Verify(user.PublicKey.Span, user.AdminSignature.Span))
            {
                return true;
            }

            return false;
        }

        public bool VerifyTrust(UserId userId)
        {
            IDatabaseUser user = database.OpenUser(userId, true);
            return VerifyTrust(user);
        }

        public IEnumerable<UserInfo> EnumerateImplicitlyTrustedUsers()
        {
            foreach (IDatabaseUser user in database.EnumerateUsers())
            {
                if (VerifyTrust(user))
                {
                    yield return UserInfo.FromDatabaseUser(user, Trust.ExplicitTrust);
                }
            }
        }

        public virtual void Dispose()
        {
        }
    }
}
