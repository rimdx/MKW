namespace MKW.Cryptography
{
    public interface IAsymmetricPublicTransformer : IDisposable
    {
        Memory<byte> Encrypt(ReadOnlySpan<byte> data);
        Memory<byte> ExportPublicKey();
        bool Verify(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature);
    }
}
