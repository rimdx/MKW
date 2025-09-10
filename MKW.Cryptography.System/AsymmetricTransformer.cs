using MKW.Cryptography.Exceptions;
using System.Security.Cryptography;

namespace MKW.Cryptography.System
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

            try
            {
                rsa.ImportSubjectPublicKeyInfo(publicKey, out _);
            }
            catch (Exception ex)
            {
                throw new InvalidKeyException(ex);
            }

            return new AsymmetricTransformer(rsa);
        }

        public static IAsymmetricPrivateTransformer Open(ReadOnlySpan<byte> publicKey, ReadOnlySpan<byte> privateKey)
        {
            RSA rsa = RSA.Create();

            try
            {
                rsa.ImportSubjectPublicKeyInfo(publicKey, out _);
                rsa.ImportPkcs8PrivateKey(privateKey, out _);
            }
            catch (CryptographicException ex)
            {
                throw new InvalidKeyException(ex);
            }

            return new AsymmetricTransformer(rsa);
        }

        public Memory<byte> Encrypt(ReadOnlySpan<byte> data)
        {
            try
            {
                return rsa.Encrypt(data, CryptographicConstants.RSA.EncryptionPadding);
            }
            catch (CryptographicException ex)
            {
                throw new AsymmetricOperationFailedException(ex);
            }
        }

        public Memory<byte> Decrypt(ReadOnlySpan<byte> data)
        {
            try
            {
                return rsa.Decrypt(data, CryptographicConstants.RSA.EncryptionPadding);
            }
            catch (CryptographicException ex)
            {
                throw new AsymmetricOperationFailedException(ex);
            }
        }

        public Memory<byte> Sign(ReadOnlySpan<byte> data)
        {
            try
            {
                return rsa.SignData(data,
                                    CryptographicConstants.RSA.SignHashAlgorithm,
                                    CryptographicConstants.RSA.SignaturePadding);
            }
            catch (CryptographicException ex)
            {
                throw new AsymmetricOperationFailedException(ex);
            }
        }

        public bool Verify(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature)
        {
            try
            {
                return rsa.VerifyData(data,
                                      signature,
                                      CryptographicConstants.RSA.SignHashAlgorithm,
                                      CryptographicConstants.RSA.SignaturePadding);
            }
            catch (CryptographicException ex)
            {
                throw new SignatureVerificationFailedException(ex);
            }
        }

        public Memory<byte> ExportPublicKey()
        {
            return rsa.ExportSubjectPublicKeyInfo();
        }

        public Memory<byte> ExportPrivateKey()
        {
            return rsa.ExportPkcs8PrivateKey();
        }

        public void Dispose()
        {
            rsa.Dispose();
        }
    }
}
