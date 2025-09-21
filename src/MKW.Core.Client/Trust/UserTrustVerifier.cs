using MKW.Core.Notify;
using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    public class UserTrustVerifier : ITrustVerifier, IDisposable
    {
        private readonly IAsymmetricPublicTransformer publicKey;
        private readonly IDatabaseUser me;

        public UserTrustVerifier(ICryptographyProvider crypto, IDatabaseUser me)
        {
            this.me = me;
            publicKey = crypto.OpenAsymmetricTransformer(me.PublicKey.Span);
        }

        public Trust GetTrust(ReadOnlySpan<byte> otherPublicKey)
        {
            if (otherPublicKey.SequenceEqual(me.PublicKey.Span))
            {
                return Trust.SelfTrust;
            }

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
