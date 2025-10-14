using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace MKW.Cryptography.BouncyCastle
{
    public class BouncyCastleCryptographyProvider : ICryptographyProvider
    {
        public BouncyCastleCryptographyProvider()
        {
        }

        public SymmetricKey CreateSymmetricKey(SymmetricAlgorithmConfiguration config)
        {
            IRandomGenerator random = CreateRandomGenerator();

            ReadOnlyMemory<byte> key = random.NextBytes(config.KeySizeBits / 8);
            ReadOnlyMemory<byte> iv = random.NextBytes(config.IVSizeBits / 8);

            return new SymmetricKey
            {
                KeyBytes = key,
                IVBytes = iv,
            };
        }

        public ISymmetricTransformer OpenSymmetricTransformer(SymmetricKey key)
        {
            return new AesGcmSymmetricTransformer(key);
        }

        public ISymmetricTransformer CreateSymmetricTransformer(SymmetricAlgorithmConfiguration config)
        {
            return OpenSymmetricTransformer(CreateSymmetricKey(config));
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

        public IUserCredentials CreateUserCredentials(string password,
                                                      PasswordDerivationConfiguration config)
        {
            IRandomGenerator random = CreateRandomGenerator();

            ReadOnlyMemory<byte> salt = random.NextBytes(config.SaltSizeBits / 8);
            ReadOnlyMemory<byte> passwordBytes = EncodingConverter.GetBytes(password);

            return config.Engine switch
            {
                PasswordDerivationEngine.Pbkdf2 => new UserCredentials(passwordBytes, salt, config),
            };
        }

        public IUserCredentials OpenUserCredentials(string password,
                                                    ReadOnlyMemory<byte> salt,
                                                    PasswordDerivationConfiguration config)
        {
            ReadOnlyMemory<byte> passwordBytes = EncodingConverter.GetBytes(password);

            return config.Engine switch
            {
                PasswordDerivationEngine.Pbkdf2 => new UserCredentials(passwordBytes, salt, config),
            };
        }

        public IRandomGenerator CreateRandomGenerator()
        {
            return new BouncyCastleRandomGenerator();
        }
    }
}
