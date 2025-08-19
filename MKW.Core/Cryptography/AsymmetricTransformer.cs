using System.Security.Cryptography;

namespace MKW.Core.Cryptography
{
    public class AsymmetricTransformer : IDisposable
    {
        private readonly RSA rsa;

        protected AsymmetricTransformer(RSA rsa)
        {
            this.rsa = rsa;
        }

        public static AsymmetricTransformer Create()
        {
            RSA rsa = RSA.Create();
            return new AsymmetricTransformer(rsa);
        }

        public static AsymmetricTransformer Open(byte[] publicKey)
        {
            RSA rsa = RSA.Create();

            rsa.ImportRSAPublicKey(publicKey, out _);

            return new AsymmetricTransformer(rsa);
        }

        public static AsymmetricTransformer Open(byte[] publicKey, byte[] privateKey)
        {
            RSA rsa = RSA.Create();

            rsa.ImportRSAPublicKey(publicKey, out _);
            rsa.ImportRSAPrivateKey(privateKey, out _);

            return new AsymmetricTransformer(rsa);
        }

        public byte[] Encrypt(byte[] data)
        {
            return rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
        }

        public byte[] Decrypt(byte[] data)
        {
            return rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
        }

        public byte[] Sign(byte[] data)
        {
            return rsa.SignData(data,
                                HashAlgorithmName.SHA256,
                                RSASignaturePadding.Pkcs1);
        }

        public bool Verify(byte[] data, byte[] signature)
        {
            return rsa.VerifyData(data,
                                  signature,
                                  HashAlgorithmName.SHA256,
                                  RSASignaturePadding.Pkcs1);
        }

        public byte[] ExportPublicKey()
        {
            return rsa.ExportRSAPublicKey();
        }

        public byte[] ExportPrivateKey()
        {
            return rsa.ExportRSAPrivateKey();
        }

        public void Dispose()
        {
            rsa.Dispose();
        }
    }
}
