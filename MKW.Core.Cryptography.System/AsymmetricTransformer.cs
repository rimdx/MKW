using System.Security.Cryptography;

namespace MKW.Core.Cryptography.System
{
    public class AsymmetricTransformer : IAsymmetricPrivateTransformer, IAsymmetricPublicTransformer, IDisposable
    {
        private readonly RSA rsa;

        protected AsymmetricTransformer(RSA rsa)
        {
            this.rsa = rsa;
        }

        public static IAsymmetricPrivateTransformer Create()
        {
            RSA rsa = RSA.Create();
            return new AsymmetricTransformer(rsa);
        }

        public static IAsymmetricPublicTransformer Open(ReadOnlySpan<byte> publicKey)
        {
            RSA rsa = RSA.Create();

            rsa.ImportRSAPublicKey(publicKey, out _);

            return new AsymmetricTransformer(rsa);
        }

        public static IAsymmetricPrivateTransformer Open(ReadOnlySpan<byte> publicKey, ReadOnlySpan<byte> privateKey)
        {
            RSA rsa = RSA.Create();

            rsa.ImportRSAPublicKey(publicKey, out _);
            rsa.ImportRSAPrivateKey(privateKey, out _);

            return new AsymmetricTransformer(rsa);
        }

        public Memory<byte> Encrypt(ReadOnlySpan<byte> data)
        {
            return rsa.Encrypt(data, CryptographicConstants.RSA.EncryptionPadding);
        }

        public Memory<byte> Decrypt(ReadOnlySpan<byte> data)
        {
            return rsa.Decrypt(data, CryptographicConstants.RSA.EncryptionPadding);
        }

        public Memory<byte> Sign(ReadOnlySpan<byte> data)
        {
            return rsa.SignData(data,
                                CryptographicConstants.RSA.SignHashAlgorithm,
                                CryptographicConstants.RSA.SignaturePadding);
        }

        public bool Verify(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature)
        {
            return rsa.VerifyData(data,
                                  signature,
                                  CryptographicConstants.RSA.SignHashAlgorithm,
                                  CryptographicConstants.RSA.SignaturePadding);
        }

        public Memory<byte> ExportPublicKey()
        {
            return rsa.ExportRSAPublicKey();
        }

        public Memory<byte> ExportPrivateKey()
        {
            return rsa.ExportRSAPrivateKey();
        }

        public void Dispose()
        {
            rsa.Dispose();
        }
    }
}
