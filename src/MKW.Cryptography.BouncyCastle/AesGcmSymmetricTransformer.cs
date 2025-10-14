using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace MKW.Cryptography.BouncyCastle
{
    internal sealed class AesGcmSymmetricTransformer : ISymmetricTransformer, IDisposable
    {
        private readonly SymmetricKey key;
        private readonly SymmetricCipher cipher;

        public AesGcmSymmetricTransformer(SymmetricKey key)
        {
            this.key = key;

            AesEngine blockCipher = new AesEngine();
            GcmBlockCipher blockCipherMode = new GcmBlockCipher(blockCipher);

            BufferedAeadBlockCipher cipher = new BufferedAeadBlockCipher(blockCipherMode);

            KeyParameter aesKey = new KeyParameter(key.KeyBytes.ToArray());
            AeadParameters parameters = new AeadParameters(aesKey, key.IVBytes.Length * 8, key.IVBytes.ToArray());

            this.cipher = new SymmetricCipher(cipher, parameters);
        }

        public Memory<byte> Decrypt(ReadOnlySpan<byte> data)
        {
            return cipher.Decrypt(data);
        }

        public Memory<byte> Encrypt(ReadOnlySpan<byte> data)
        {
            return cipher.Encrypt(data);
        }

        public Memory<byte> ExportIV()
        {
            return key.IVBytes.ToArray();
        }

        public Memory<byte> ExportKey()
        {
            return key.KeyBytes.ToArray();
        }

        public void Dispose()
        {
        }
    }
}
