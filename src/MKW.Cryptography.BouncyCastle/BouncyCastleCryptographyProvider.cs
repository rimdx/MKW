namespace MKW.Cryptography.BouncyCastle
{
    public class BouncyCastleCryptographyProvider : ICryptographyProvider
    {
        public BouncyCastleCryptographyProvider()
        {
        }

        public ISymmetricTransformer OpenSymmetricTransformer(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv)
        {
            return OpenSymmetricTransformer(key, iv, CommonCryptographyAlgorithms.Aes128Gcm);
        }

        public ISymmetricTransformer OpenSymmetricTransformer(ReadOnlySpan<byte> key,
                                                              ReadOnlySpan<byte> iv,
                                                              SymmetricAlgorithmConfiguration config)
        {
            if (key.Length != config.KeySizeBits / 8)
            {
                throw new Exceptions.InvalidKeyException($"Symmetric key length expected to be {config.KeySizeBits} bits.");
            }

            if (iv.Length != config.IVSizeBits / 8)
            {
                throw new Exceptions.InvalidKeyException($"Symmetric IV length expected to be {config.IVSizeBits} bits.");
            }

            return config.Engine switch
            {
                SymmetricAlgorithmEngine.AesGcm => new AesGcmSymmetricTransformer(key, iv, config),
            };
        }

        public ISymmetricTransformer CreateSymmetricTransformer(SymmetricAlgorithmConfiguration config)
        {
            IRandomGenerator random = CreateRandomGenerator();

            byte[] key = random.NextBytes(config.KeySizeBits / 8);
            byte[] iv = random.NextBytes(config.IVSizeBits / 8);

            return OpenSymmetricTransformer(key, iv, config);
        }

        public ISymmetricTransformer CreateSymmetricTransformer()
        {
            return CreateSymmetricTransformer(CommonCryptographyAlgorithms.Aes128Gcm);
        }

        public IAsymmetricPrivateTransformer CreateAsymmetricTransformer()
        {
            return AsymmetricTransformer.Create();
        }

        public IAsymmetricPublicTransformer OpenAsymmetricTransformer(ReadOnlySpan<byte> publicKey)
        {
            return AsymmetricTransformer.Open(publicKey);
        }

        public IAsymmetricPrivateTransformer OpenAsymmetricTransformer(ReadOnlySpan<byte> publicKey, ReadOnlySpan<byte> privateKey)
        {
            return AsymmetricTransformer.Open(publicKey, privateKey);
        }

        public IUserCredentials CreateUserCredentials(string password)
        {
            return UserCredentials.Create(password);
        }

        public IUserCredentials OpenUserCredentials(string password, ReadOnlyMemory<byte> salt)
        {
            return UserCredentials.Open(password, salt);
        }

        public IRandomGenerator CreateRandomGenerator()
        {
            return new BouncyCastleRandomGenerator();
        }
    }
}
