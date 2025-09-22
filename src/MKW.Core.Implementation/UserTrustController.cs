using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Implementation
{
    public class UserTrustController
        : UserTrustProvider
        , ITrustProvider
        , IDisposable
    {
        protected readonly IAsymmetricPrivateTransformer privateKey;

        public UserTrustController(IDatabase database,
                                   ICryptographyProvider crypto,
                                   IAsymmetricPrivateTransformer privateKey)
            : base(database, crypto, privateKey, privateKey)
        {
            this.privateKey = privateKey;
        }

        public void AddTrust(IDatabaseUser user)
        {
            user.AdminSignature = privateKey.Sign(user.PublicKey.Span);
            user.Save();
        }

        public override void Dispose()
        {
            // no-op

            // In this case, the UserTrustProvider.key is actually managed by
            // caller, since the user is given by a reference.
        }
    }
}
