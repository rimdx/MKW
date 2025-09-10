using MKW.Common;
using MKW.Core.Cryptography;
using MKW.Core.Cryptography.Exceptions;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace MKW.Cryptography.BouncyCastle
{
    internal class SymmetricTransformer : ISymmetricTransformer, IDisposable
    {
        private readonly ICipherParameters parameters;

        private readonly IBufferedCipher cipher;

        private readonly ReadOnlyMemory<byte> key;
        private readonly ReadOnlyMemory<byte> iv;

        private SymmetricTransformer(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv)
        {
            if (key.Length != 16)
            {
                throw new Exceptions.InvalidKeyException($"Symmetric key length must be 16 bytes.");
            }

            if (iv.Length != 16)
            {
                throw new Exceptions.InvalidKeyException($"Symmetric IV length must be 16 bytes.");
            }

            cipher = CipherUtilities.GetCipher(NistObjectIdentifiers.IdAes128Cbc);
            parameters = new ParametersWithIV(new KeyParameter(key.ToArray()), iv.ToArray());

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
