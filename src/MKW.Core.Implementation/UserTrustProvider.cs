using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Implementation
{
    public class UserTrustProvider : IDisposable
    {
        protected readonly IDatabase database;
        protected readonly ICryptographyProvider crypto;
        private readonly IAsymmetricPublicTransformer meKey;
        private readonly IAsymmetricPublicTransformer adminKey;

        public UserTrustProvider(IDatabase database,
                                 ICryptographyProvider crypto,
                                 IAsymmetricPublicTransformer meKey,
                                 IAsymmetricPublicTransformer adminKey)
        {
            this.database = database;
            this.crypto = crypto;
            this.meKey = meKey;
            this.adminKey = adminKey;
        }

        private bool VerifyTrust(DatabaseUser user)
        {
            // trust ourselves
            if (user.PublicKey.Payload.Span.SequenceEqual(meKey.ExportPublicKey().Span))
            {
                return true;
            }

            // trust admin
            // TODO: verify admin
            if (user.PublicKey.Payload.Span.SequenceEqual(adminKey.ExportPublicKey().Span))
            {
                return true;
            }

            // otherwise verify admin trust to this user
            if (adminKey.Verify(user.PublicKey.Payload.Span, user.PublicKey.Signature.Span))
            {
                return true;
            }

            return false;
        }

        public bool VerifyTrust(UserId userId)
        {
            return VerifyTrust(database.OpenUser(userId));
        }

        public IEnumerable<UserId> EnumerateTrustedUsers()
        {
            foreach (DatabaseUser user in database.EnumerateUsers())
            {
                if (VerifyTrust(user))
                {
                    yield return user.Id;
                }
            }
        }

        public virtual void Dispose()
        {
        }
    }
}
