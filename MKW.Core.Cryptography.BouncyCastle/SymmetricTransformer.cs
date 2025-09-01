using MKW.Core.Common;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace MKW.Core.Cryptography.BouncyCastle
{
    internal class SymmetricTransformer : ISymmetricTransformer, IDisposable
    {
        private readonly ICipherParameters parameters;

        private readonly IBufferedCipher cipher;

        private readonly ReadOnlyMemory<byte> key;
        private readonly ReadOnlyMemory<byte> iv;

        private SymmetricTransformer(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv)
        {
            cipher = CipherUtilities.GetCipher(NistObjectIdentifiers.IdAes128Cbc);
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

        private void Init(bool forEncryption)
        {
            cipher.Init(forEncryption, parameters);
        }

        public Memory<byte> Decrypt(ReadOnlySpan<byte> data)
        {
            Init(false);

            using MemoryStream output = new MemoryStream();

            using (CipherStream cipherStream = new CipherStream(new StreamDisown(output),
                                                                null, cipher))
            {
                cipherStream.Write(data);
            }

            return output.ToArray();
        }

        public Memory<byte> Encrypt(ReadOnlySpan<byte> data)
        {
            Init(true);

            using MemoryStream output = new MemoryStream();

            using (CipherStream cipherStream = new CipherStream(new StreamDisown(output),
                                                                null, cipher))
            {
                cipherStream.Write(data);
            }

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
