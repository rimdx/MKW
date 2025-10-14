using MKW.Cryptography.Exceptions;
using System.Security.Cryptography;

namespace MKW.Cryptography.System
{
    public class SymmetricTransformer : ISymmetricTransformer, IDisposable
    {
        private readonly Aes aes;

        protected SymmetricTransformer(Aes aes)
        {
            this.aes = aes;
        }

        public static SymmetricKey CreateKey()
        {
            Aes aes = Aes.Create();

            aes.GenerateKey();
            aes.GenerateIV();

            return new SymmetricKey
            {
                Engine = SymmetricAlgorithmEngine.AesGcm,
                KeyBytes = aes.Key,
                IVBytes = aes.IV,
            };
        }

        public static ISymmetricTransformer Open(SymmetricKey key)
        {
            Aes aes = Aes.Create();

            try
            {
                aes.Key = key.KeyBytes.ToArray(); /* copy */
                aes.IV = key.IVBytes.ToArray(); /* copy */
            }
            catch (CryptographicException ex)
            {
                throw new InvalidKeyException(ex);
            }

            return new SymmetricTransformer(aes /* move */);
        }

        public Memory<byte> Encrypt(ReadOnlySpan<byte> data)
        {
            try
            {
                using MemoryStream output = new MemoryStream();
                using ICryptoTransform encryptor = aes.CreateEncryptor();
                using CryptoStream encryptorStream = new CryptoStream(output, encryptor, CryptoStreamMode.Write);

                encryptorStream.Write(data);
                encryptorStream.FlushFinalBlock();

                return output.ToArray();
            }
            catch (CryptographicException ex)
            {
                throw new SymmetricOperationFailedException(ex);
            }
        }

        public Memory<byte> Decrypt(ReadOnlySpan<byte> data)
        {
            try
            {
                using MemoryStream output = new MemoryStream();
                using ICryptoTransform decryptor = aes.CreateDecryptor();
                using CryptoStream decryptorStream = new CryptoStream(output, decryptor, CryptoStreamMode.Write);

                decryptorStream.Write(data);
                decryptorStream.FlushFinalBlock();

                return output.ToArray();
            }
            catch (CryptographicException ex)
            {
                throw new SymmetricOperationFailedException(ex);
            }
        }

        public Memory<byte> ExportIV()
        {
            return aes.IV;
        }

        public Memory<byte> ExportKey()
        {
            return aes.Key;
        }

        public void Dispose()
        {
            aes.Dispose();
        }
    }
}
