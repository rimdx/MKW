using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class UserTrustVerifier : ITrustVerifier, IDisposable
    {
        private readonly IAsymmetricPublicTransformer publicKey;
        private readonly IDatabaseUser me;

        public UserTrustVerifier(IDatabaseUser me)
        {
            this.me = me;
            publicKey = AsymmetricTransformer.Open(me.PublicKey.Span);
        }

        public Trust GetTrust(ReadOnlySpan<byte> otherPublicKey)
        {
            foreach (ReadOnlyMemory<byte> trust in me.EnumerateTrust())
            {
                if (publicKey.Verify(otherPublicKey, trust.Span))
                {
                    return Trust.ExplicitTrust;
                }
            }

            return Trust.None;
        }

        public void Dispose()
        {
            publicKey.Dispose();
        }
    }
}
