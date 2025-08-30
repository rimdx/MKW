namespace MKW.Core.Cryptography
{
    public interface IAsymmetricPrivateTransformer : IAsymmetricPublicTransformer, IDisposable
    {
        Memory<byte> Decrypt(ReadOnlySpan<byte> data);
        Memory<byte> ExportPrivateKey();
        Memory<byte> Sign(ReadOnlySpan<byte> data);
    }
}
