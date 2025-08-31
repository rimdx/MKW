
namespace MKW.Core.Cryptography.BouncyCastle
{
    internal class SymmetricTransformer : ISymmetricTransformer, IDisposable
    {
        private SymmetricTransformer()
        {
        }

        public static SymmetricTransformer Create()
        {
            return new SymmetricTransformer();
        }

        public static SymmetricTransformer Open(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv)
        {
            return new SymmetricTransformer();
        }

        public Memory<byte> Decrypt(ReadOnlySpan<byte> data)
        {
            throw new NotImplementedException();
        }

        public Memory<byte> Encrypt(ReadOnlySpan<byte> data)
        {
            throw new NotImplementedException();
        }

        public Memory<byte> ExportIV()
        {
            throw new NotImplementedException();
        }

        public Memory<byte> ExportKey()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
