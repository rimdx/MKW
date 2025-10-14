using MKW.Common;
using MKW.Cryptography.Exceptions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace MKW.Cryptography.BouncyCastle
{
    internal sealed class AesGcmSymmetricTransformer : ISymmetricTransformer, IDisposable
    {
        private readonly SymmetricKey key;

        private readonly IBlockCipher blockCipher;
        private readonly IAeadBlockCipher blockCipherMode;

        private readonly IBufferedCipher cipher;

        private readonly ICipherParameters parameters;

        public AesGcmSymmetricTransformer(SymmetricKey key)
        {
            this.key = key;

            blockCipher = new AesEngine();
            blockCipherMode = new GcmBlockCipher(blockCipher);

            cipher = new BufferedAeadBlockCipher(blockCipherMode);

            KeyParameter aesKey = new KeyParameter(key.KeyBytes.ToArray());
            parameters = new AeadParameters(aesKey, key.IVBytes.Length * 8, key.IVBytes.ToArray());
        }

        public Memory<byte> Decrypt(ReadOnlySpan<byte> data)
        {
            try
            {
                cipher.Init(false, parameters);

                using MemoryStream output = new MemoryStream();

                using (CipherStream cipherStream = new CipherStream(new StreamDisown(output),
                                                                    null, cipher))
                {
                    cipherStream.Write(data);
                }

                return output.ToArray();
            }
            catch (CryptoException ex)
            {
                throw new SymmetricOperationFailedException(ex);
            }
        }

        public Memory<byte> Encrypt(ReadOnlySpan<byte> data)
        {
            try
            {
                cipher.Init(true, parameters);

                using MemoryStream output = new MemoryStream();

                using (CipherStream cipherStream = new CipherStream(new StreamDisown(output),
                                                                    null, cipher))
                {
                    cipherStream.Write(data);
                }

                return output.ToArray();
            }
            catch (CryptoException ex)
            {
                throw new SymmetricOperationFailedException(ex);
            }
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
