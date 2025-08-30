using System.Security.Cryptography;

namespace MKW.Core.Cryptography
{
    public class SymmetricTransformer : ISymmetricTransformer, IDisposable
    {
        private readonly Aes aes;

        protected SymmetricTransformer(Aes aes)
        {
            this.aes = aes;
        }

        public static SymmetricTransformer Create()
        {
            Aes aes = Aes.Create();

            aes.GenerateKey();
            aes.GenerateIV();

            return new SymmetricTransformer(aes /* move */);
        }

        public static SymmetricTransformer Open(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv)
        {
            Aes aes = Aes.Create();

            aes.Key = key.ToArray(); /* copy */
            aes.IV = iv.ToArray(); /* copy */

            return new SymmetricTransformer(aes /* move */);
        }

        public Memory<byte> Encrypt(ReadOnlySpan<byte> data)
        {
            using MemoryStream output = new MemoryStream();
            using ICryptoTransform encryptor = aes.CreateEncryptor();
            using CryptoStream encryptorStream = new CryptoStream(output, encryptor, CryptoStreamMode.Write);

            encryptorStream.Write(data);
            encryptorStream.FlushFinalBlock();

            return output.ToArray();
        }

        public Memory<byte> Decrypt(ReadOnlySpan<byte> data)
        {
            using MemoryStream output = new MemoryStream();
            using ICryptoTransform decryptor = aes.CreateDecryptor();
            using CryptoStream decryptorStream = new CryptoStream(output, decryptor, CryptoStreamMode.Write);

            decryptorStream.Write(data);
            decryptorStream.FlushFinalBlock();

            return output.ToArray();
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
