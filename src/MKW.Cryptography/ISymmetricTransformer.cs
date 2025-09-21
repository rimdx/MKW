namespace MKW.Cryptography
{
    public interface ISymmetricTransformer : IDisposable
    {
        Memory<byte> Encrypt(ReadOnlySpan<byte> data);
        Memory<byte> Decrypt(ReadOnlySpan<byte> data);

        Memory<byte> ExportIV();
        Memory<byte> ExportKey();
    }
}