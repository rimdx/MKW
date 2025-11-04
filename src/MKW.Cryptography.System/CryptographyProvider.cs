// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Security.Cryptography;

namespace MKW.Cryptography.System
{
    public class CryptographyProvider : IDisposable, ICryptographyProvider
    {
        public CryptographyProvider()
        {
        }

        public AsymmetricPrivateKey CreateAsymmetricKey(AsymmetricAlgorithmConfiguration config)
        {
            return AsymmetricTransformer.CreateKey();
        }

        public AsymmetricPublicKey DecodePkcsPublicKey(ReadOnlySpan<byte> data)
        {
            // workaround
            using RSA rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(data, out _);
            return AsymmetricPublicKeyExtensions.FromParameter(rsa.ExportParameters(false));
        }

        public AsymmetricPrivateKey DecodePkcsPrivateKey(ReadOnlySpan<byte> data)
        {
            // workaround
            using RSA rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(data, out _);
            return AsymmetricPrivateKeyExtensions.FromParameter(rsa.ExportParameters(true));
        }

        public Memory<byte> EncodePkcsPublicKey(AsymmetricPublicKey key)
        {
            // workaround
            using RSA rsa = RSA.Create();
            rsa.ImportParameters(key.GetParameter());
            return rsa.ExportSubjectPublicKeyInfo();
        }

        public Memory<byte> EncodePkcsPrivateKey(AsymmetricPrivateKey key)
        {
            // workaround
            using RSA rsa = RSA.Create();
            rsa.ImportParameters(key.GetParameter());
            return rsa.ExportPkcs8PrivateKey();
        }

        public IAsymmetricPublicTransformer OpenAsymmetricTransformer(AsymmetricPublicKey publicKey,
                                                                      AsymmetricAlgorithmConfiguration config)
        {
            return AsymmetricTransformer.Open(publicKey);
        }

        public IAsymmetricPrivateTransformer OpenAsymmetricTransformer(AsymmetricPrivateKey privateKey,
                                                                       AsymmetricAlgorithmConfiguration config)
        {
            return AsymmetricTransformer.Open(privateKey);
        }

        public SymmetricKey CreateSymmetricKey(SymmetricAlgorithmConfiguration config)
        {
            return SymmetricTransformer.CreateKey();
        }

        public ISymmetricTransformer OpenSymmetricTransformer(SymmetricKey key)
        {
            return key.Visit(new SymmetricTransformerFactoryVisitor());
        }

        public IUserCredentials CreateUserCredentials(string password,
                                                      PasswordDerivationConfiguration config)
        {
            return UserCredentials.Create(password);
        }

        public IUserCredentials OpenUserCredentials(string password,
                                                    ReadOnlyMemory<byte> salt,
                                                    PasswordDerivationConfiguration config)
        {
            return UserCredentials.Open(password, salt);
        }

        public IRandomGenerator CreateRandomGenerator()
        {
            return new SystemRandomGenerator();
        }

        public void Dispose()
        {
        }

        public SymmetricKey OpenSymmetricKey(SymmetricAlgorithmConfiguration config,
                                             ReadOnlyMemory<byte> key,
                                             ReadOnlyMemory<byte> iv)
        {
            return config.Engine switch
            {
                SymmetricAlgorithmEngine.AesGcm => new SymmetricKeyAesGcm
                {
                    KeyBytes = key,
                    IVBytes = iv,
                },
                SymmetricAlgorithmEngine.AesOpenPgpCfb => throw new InvalidCastException(),
            };
        }
    }
}
