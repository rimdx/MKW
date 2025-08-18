using System.Security.Cryptography;

namespace MKW.Core
{
    public class SymmetricTransformer : IDisposable
    {
        private readonly Aes aes;

        protected SymmetricTransformer(byte[] key, byte[]? iv)
        {
            aes = Aes.Create();

            aes.Key = key;

            if (iv == null)
                aes.GenerateIV();
            else
                aes.IV = iv;
        }

        public static SymmetricTransformer Create(byte[] key)
        {
            return new SymmetricTransformer(key, null);
        }

        public static SymmetricTransformer Open(byte[] key, byte[] iv)
        {
            return new SymmetricTransformer(key, iv);
        }

        public byte[] Encrypt(byte[] data)
        {
            using MemoryStream output = new MemoryStream();
            using ICryptoTransform encryptor = aes.CreateEncryptor();
            using CryptoStream encryptorStream = new CryptoStream(output, encryptor, CryptoStreamMode.Write);

            encryptorStream.Write(data);
            encryptorStream.FlushFinalBlock();

            return output.ToArray();
        }

        public byte[] Decrypt(byte[] data)
        {
            using MemoryStream input = new MemoryStream(data);
            using ICryptoTransform decryptor = aes.CreateDecryptor();
            using CryptoStream decryptorStream = new CryptoStream(input, decryptor, CryptoStreamMode.Read);
            using MemoryStream output = new MemoryStream(input.Capacity);

            decryptorStream.CopyTo(output);

            return output.ToArray();
        }

        public byte[] ExportIV()
        {
            return aes.IV;
        }

        public void Dispose()
        {
            aes.Dispose();
        }
    }
}
