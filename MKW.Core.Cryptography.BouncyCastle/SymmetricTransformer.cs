using MKW.Core.Common;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace MKW.Core.Cryptography.BouncyCastle
{
    internal class SymmetricTransformer : ISymmetricTransformer, IDisposable
    {
        private readonly IBufferedCipher cipher;
        private readonly ICipherParameters parameters;

        private readonly ReadOnlyMemory<byte> key;
        private readonly ReadOnlyMemory<byte> iv;

        private SymmetricTransformer(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv)
        {
            AesEngine engine = new AesEngine();
            Pkcs7Padding padding = new Pkcs7Padding();
            CbcBlockCipher mode = new CbcBlockCipher(engine);
            cipher = new PaddedBufferedBlockCipher(mode, padding);

            parameters = new ParametersWithIV(new KeyParameter(key), iv);

            this.key = key.ToArray();
            this.iv = iv.ToArray();
        }

        public static SymmetricTransformer Create()
        {
            SecureRandom random = new SecureRandom();

            return new SymmetricTransformer(SecureRandom.GetNextBytes(random, 16),
                                            SecureRandom.GetNextBytes(random, 16));
        }

        public static SymmetricTransformer Open(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv)
        {
            return new SymmetricTransformer(key, iv);
        }

        public Memory<byte> Decrypt(ReadOnlySpan<byte> data)
        {
            using MemoryStream output = new MemoryStream();
            using CipherStream cipherStream = new CipherStream(new StreamDisown(output),
                                                               null, cipher);

            cipher.Init(false, parameters);

            cipherStream.Write(data);
            cipherStream.Close();

            return output.ToArray();
        }

        public Memory<byte> Encrypt(ReadOnlySpan<byte> data)
        {
            using MemoryStream output = new MemoryStream();
            using CipherStream cipherStream = new CipherStream(new StreamDisown(output),
                                                               null, cipher);

            cipher.Init(true, parameters);

            cipherStream.Write(data);
            cipherStream.Close();

            return output.ToArray();
        }

        public Memory<byte> ExportIV()
        {
            return iv.ToArray();
        }

        public Memory<byte> ExportKey()
        {
            return key.ToArray();
        }

        public void Dispose()
        {
        }
    }
}
