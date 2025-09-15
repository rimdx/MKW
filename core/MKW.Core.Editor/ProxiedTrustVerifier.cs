using MKW.Core.Client;
using MKW.Core.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Editor
{
    internal class ProxiedTrustVerifier : ITrustVerifier, IDisposable
    {
        private readonly ITrustVerifier proxy;
        private readonly Dictionary<UserId, Trust> edits;

        public ProxiedTrustVerifier(ITrustVerifier proxy, Dictionary<UserId, Trust> edits)
        {
            this.proxy = proxy;
            this.edits = edits;
        }

        public Trust GetTrust(ReadOnlySpan<byte> otherPublicKey)
        {
            return proxy.GetTrust(otherPublicKey);
        }

        public void Dispose()
        {
            proxy?.Dispose();
        }
    }
}
