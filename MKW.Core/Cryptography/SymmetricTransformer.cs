using System.Security.Cryptography;

namespace MKW.Core.Cryptography
{
    public class SymmetricTransformer : IDisposable
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

        public static SymmetricTransformer Open(byte[] key, byte[] iv)
        {
            Aes aes = Aes.Create();

            aes.Key = key;
            aes.IV = iv;

            return new SymmetricTransformer(aes /* move */);
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

        public byte[] ExportKey()
        {
            return aes.Key;
        }

        public void Dispose()
        {
            aes.Dispose();
        }
    }
}
