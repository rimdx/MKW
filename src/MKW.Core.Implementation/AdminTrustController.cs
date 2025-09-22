using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Implementation
{
    public class AdminTrustController : IDisposable
    {
        protected readonly IAsymmetricPrivateTransformer adminKey;

        public AdminTrustController(IAsymmetricPrivateTransformer adminKey)
        {
            this.adminKey = adminKey;
        }

        public void AddTrust(IDatabaseUser user)
        {
            user.AdminSignature = adminKey.Sign(user.PublicKey.Span);
            user.Save();
        }

        public void Dispose()
        {
            // no-op

            // In this case, the UserTrustProvider.key is actually managed by
            // caller, since the user is given by a reference.
        }
    }
}
