using MKW.Core.Client.Notify;

namespace MKW.Core.Client
{
    public interface ITrustVerifier : IDisposable
    {
        Trust GetTrust(ReadOnlySpan<byte> otherPublicKey);
    }
}
