// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

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

        public static AsymmetricPrivateKey CreateKey()
        {
            using RSA rsa = RSA.Create();
            RSAParameters parameters = rsa.ExportParameters(true);
            return AsymmetricPrivateKeyExtensions.FromParameter(parameters);
        }

        public static IAsymmetricPublicTransformer Open(AsymmetricPublicKey publicKey)
        {
            RSA rsa = RSA.Create();

            try
            {
                rsa.ImportParameters(publicKey.GetParameter());
            }
            catch (Exception ex)
            {
                throw new InvalidKeyException(ex);
            }

            return new AsymmetricTransformer(rsa);
        }

        public static IAsymmetricPrivateTransformer Open(AsymmetricPrivateKey privateKey)
        {
            RSA rsa = RSA.Create();

            try
            {
                rsa.ImportParameters(privateKey.GetParameter());
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
