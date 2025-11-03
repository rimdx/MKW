// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Client
{
    public sealed class ClientCryptography
    {
        private readonly ICryptographyProvider provider;
        private readonly DatabaseConfiguration config;

        public ClientCryptography(ICryptographyProvider provider,
                                  DatabaseConfiguration config)
        {
            this.provider = provider;
            this.config = config;
        }

        public AsymmetricPrivateKey CreateAsymmetricKey()
        {
            return provider.CreateAsymmetricKey(config.PreferredPublicKeyAlgorithm);
        }

        public IAsymmetricPublicTransformer OpenAsymmetricTransformer(AsymmetricPublicKey publicKey)
        {
            return provider.OpenAsymmetricTransformer(publicKey, config.PreferredPublicKeyAlgorithm);
        }

        public IAsymmetricPrivateTransformer OpenAsymmetricTransformer(AsymmetricPrivateKey privateKey)
        {
            return provider.OpenAsymmetricTransformer(privateKey, config.PreferredPublicKeyAlgorithm);
        }

        public AsymmetricPublicKey DecodePkcsPublicKey(ReadOnlySpan<byte> data)
        {
            return provider.DecodePkcsPublicKey(data);
        }

        public AsymmetricPrivateKey DecodePkcsPrivateKey(ReadOnlySpan<byte> data)
        {
            return provider.DecodePkcsPrivateKey(data);
        }

        public Memory<byte> EncodePkcsPublicKey(AsymmetricPublicKey key)
        {
            return provider.EncodePkcsPublicKey(key);
        }

        public Memory<byte> EncodePkcsPrivateKey(AsymmetricPrivateKey key)
        {
            return provider.EncodePkcsPrivateKey(key);
        }

        public SymmetricKey CreateSymmetricKey()
        {
            return provider.CreateSymmetricKey(config.PreferredSymmetricAlgorithm);
        }

        public SymmetricKey OpenSymmetricKey(ReadOnlyMemory<byte> key,
                                             ReadOnlyMemory<byte> iv)
        {
            return provider.OpenSymmetricKey(config.PreferredSymmetricAlgorithm, key, iv);
        }

        public ISymmetricTransformer OpenSymmetricTransformer(SymmetricKey symkey)
        {
            return provider.OpenSymmetricTransformer(symkey);
        }

        public IUserCredentials CreateUserCredentials(string password)
        {
            return provider.CreateUserCredentials(password, config.PreferredStringToKeyAlgorithm);
        }

        public IUserCredentials OpenUserCredentials(string password, ReadOnlyMemory<byte> salt)
        {
            return provider.OpenUserCredentials(password, salt, config.PreferredStringToKeyAlgorithm);
        }
    }
}
