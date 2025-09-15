using MKW.Core.Notify;

namespace MKW.Core.Client
{
    public interface ITrustVerifier : IDisposable
    {
        Trust GetTrust(ReadOnlySpan<byte> otherPublicKey);
    }
}
