using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

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

        public IAsymmetricPrivateTransformer CreateAsymmetricTransformer(AsymmetricAlgorithmConfiguration config)
        {
            return config.Engine switch
            {
                AsymmetricAlgorithmEngine.Rsa => CreateRsaTransformer(config),
            };
        }

        private RsaAsymmetricTransformer CreateRsaTransformer(AsymmetricAlgorithmConfiguration config)
        {
            SecureRandom random = new SecureRandom();

            IAsymmetricCipherKeyPairGenerator keyPairGen = GeneratorUtilities.GetKeyPairGenerator("RSA");

            keyPairGen.Init(new KeyGenerationParameters(random, config.StrengthBits));

            AsymmetricCipherKeyPair key = keyPairGen.GenerateKeyPair();

            return new RsaAsymmetricTransformer(key.Public, key.Private, config);
        }

        public IAsymmetricPrivateTransformer CreateAsymmetricTransformer()
        {
            return CreateAsymmetricTransformer(CommonCryptographyAlgorithms.Rsa2048);
        }

        public IAsymmetricPublicTransformer OpenAsymmetricTransformer(ReadOnlySpan<byte> publicKey,
                                                                      AsymmetricAlgorithmConfiguration config)
        {
            try
            {
                return config.Engine switch
                {
                    AsymmetricAlgorithmEngine.Rsa => new RsaAsymmetricTransformer(
                        PublicKeyFactory.CreateKey(publicKey.ToArray()),
                        null,
                        config),
                };
            }
            catch (ArgumentException ex)
            {
                throw new Exceptions.InvalidKeyException(ex);
            }
        }

        public IAsymmetricPrivateTransformer OpenAsymmetricTransformer(ReadOnlySpan<byte> publicKey,
                                                                       ReadOnlySpan<byte> privateKey,
                                                                       AsymmetricAlgorithmConfiguration config)
        {
            try
            {
                return config.Engine switch
                {
                    AsymmetricAlgorithmEngine.Rsa => new RsaAsymmetricTransformer(
                        PublicKeyFactory.CreateKey(publicKey.ToArray()),
                        PrivateKeyFactory.CreateKey(privateKey.ToArray()),
                        config),
                };
            }
            catch (ArgumentException ex)
            {
                throw new Exceptions.InvalidKeyException(ex);
            }
        }

        public IAsymmetricPrivateTransformer OpenAsymmetricTransformer(ReadOnlySpan<byte> publicKey, ReadOnlySpan<byte> privateKey)
        {
            return OpenAsymmetricTransformer(publicKey, privateKey, CommonCryptographyAlgorithms.Rsa2048);
        }

        public IAsymmetricPublicTransformer OpenAsymmetricTransformer(ReadOnlySpan<byte> publicKey)
        {
            return OpenAsymmetricTransformer(publicKey, CommonCryptographyAlgorithms.Rsa2048);
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
