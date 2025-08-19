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
            return rsa.Encrypt(data, CryptographicConstants.RSA.EncryptionPadding);
        }

        public byte[] Decrypt(byte[] data)
        {
            return rsa.Decrypt(data, CryptographicConstants.RSA.EncryptionPadding);
        }

        public byte[] Sign(byte[] data)
        {
            return rsa.SignData(data,
                                CryptographicConstants.RSA.SignHashAlgorithm,
                                CryptographicConstants.RSA.SignaturePadding);
        }

        public bool Verify(byte[] data, byte[] signature)
        {
            return rsa.VerifyData(data,
                                  signature,
                                  CryptographicConstants.RSA.SignHashAlgorithm,
                                  CryptographicConstants.RSA.SignaturePadding);
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
