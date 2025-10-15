using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace MKW.Cryptography.BouncyCastle
{
    internal sealed class AesOpenPgpTransformer : ISymmetricTransformer, IDisposable
    {
        private readonly SymmetricKey key;
        private readonly SymmetricCipher cipher;

        public AesOpenPgpTransformer(SymmetricKey key)
        {
            this.key = key;

            AesEngine blockCipher = new AesEngine();
            OpenPgpCfbBlockCipher blockCipherMode = new OpenPgpCfbBlockCipher(blockCipher);
            BufferedBlockCipher cipher = new BufferedBlockCipher(blockCipherMode);

            KeyParameter aesKey = new KeyParameter(key.KeyBytes.ToArray());
            ParametersWithIV parameters = new ParametersWithIV(aesKey, key.IVBytes.ToArray());

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
            return key.KeyBytes.ToArray();
        }

        public Memory<byte> ExportKey()
        {
            return key.IVBytes.ToArray();
        }

        public void Dispose()
        {
            // no-op
        }
    }
}
