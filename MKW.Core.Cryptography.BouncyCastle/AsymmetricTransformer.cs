namespace MKW.Core.Cryptography.BouncyCastle
{
    public class AsymmetricTransformer : IAsymmetricPrivateTransformer, IAsymmetricPublicTransformer, IDisposable
    {
        public AsymmetricTransformer()
        {
        }

        public static AsymmetricTransformer Create()
        {
            throw new NotImplementedException();
        }

        public static AsymmetricTransformer Open(ReadOnlySpan<byte> publicKey)
        {
            throw new NotImplementedException();
        }

        public static AsymmetricTransformer Open(ReadOnlySpan<byte> publicKey, ReadOnlySpan<byte> privateKey)
        {
            throw new NotImplementedException();
        }

        public Memory<byte> Encrypt(ReadOnlySpan<byte> data)
        {
            throw new NotImplementedException();
        }

        public Memory<byte> Decrypt(ReadOnlySpan<byte> data)
        {
            throw new NotImplementedException();
        }

        public Memory<byte> ExportPrivateKey()
        {
            throw new NotImplementedException();
        }

        public Memory<byte> ExportPublicKey()
        {
            throw new NotImplementedException();
        }

        public Memory<byte> Sign(ReadOnlySpan<byte> data)
        {
            throw new NotImplementedException();
        }

        public bool Verify(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
