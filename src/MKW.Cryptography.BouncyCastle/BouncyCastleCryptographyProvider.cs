// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

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
                Engine = config.Engine,
                KeyBytes = key,
                IVBytes = iv,
            };
        }

        public ISymmetricTransformer OpenSymmetricTransformer(SymmetricKey key)
        {
            return key.Engine switch
            {
                SymmetricAlgorithmEngine.AesGcm => new AesGcmSymmetricTransformer(key),
                SymmetricAlgorithmEngine.AesOpenPgpCfb => new AesOpenPgpTransformer(key),
            };
        }

        public ISymmetricTransformer CreateSymmetricTransformer(SymmetricAlgorithmConfiguration config)
        {
            return OpenSymmetricTransformer(CreateSymmetricKey(config));
        }

        public ISymmetricTransformer CreateSymmetricTransformer()
        {
            return CreateSymmetricTransformer(CommonCryptographyAlgorithms.Aes128Gcm);
        }

        public AsymmetricPrivateKey CreateAsymmetricKey(AsymmetricAlgorithmConfiguration config)
        {
            SecureRandom random = new SecureRandom();

            RsaKeyPairGenerator keyPairGen = new RsaKeyPairGenerator();

            keyPairGen.Init(new KeyGenerationParameters(random, config.StrengthBits));

            AsymmetricCipherKeyPair key = keyPairGen.GenerateKeyPair();
            return AsymmetricPrivateKeyExtensions.FromParameter((RsaPrivateCrtKeyParameters)key.Private);
        }

        public AsymmetricPublicKey DecodePkcsPublicKey(ReadOnlySpan<byte> data)
        {
            AsymmetricKeyParameter decoded = PublicKeyFactory.CreateKey(data.ToArray());
            return AsymmetricPublicKeyExtensions.FromParameter((RsaKeyParameters)decoded);
        }

        public AsymmetricPrivateKey DecodePkcsPrivateKey(ReadOnlySpan<byte> data)
        {
            AsymmetricKeyParameter decoded = PrivateKeyFactory.CreateKey(data.ToArray());
            return AsymmetricPrivateKeyExtensions.FromParameter((RsaPrivateCrtKeyParameters)decoded);
        }

        public Memory<byte> EncodePkcsPublicKey(AsymmetricPublicKey key)
        {
            return SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(key.GetParameter()).GetEncoded();
        }

        public Memory<byte> EncodePkcsPrivateKey(AsymmetricPrivateKey key)
        {
            return PrivateKeyInfoFactory.CreatePrivateKeyInfo(key.GetParameter()).GetEncoded();
        }

        public IAsymmetricPublicTransformer OpenAsymmetricTransformer(AsymmetricPublicKey publicKey,
                                                                      AsymmetricAlgorithmConfiguration config)
        {
            try
            {
                return new RsaAsymmetricTransformer(publicKey, null, config);
            }
            catch (ArgumentException ex)
            {
                throw new Exceptions.InvalidKeyException(ex);
            }
        }

        public IAsymmetricPrivateTransformer OpenAsymmetricTransformer(AsymmetricPrivateKey privateKey,
                                                                       AsymmetricAlgorithmConfiguration config)
        {
            try
            {
                return new RsaAsymmetricTransformer(privateKey.GetPublicKey(), privateKey, config);
            }
            catch (ArgumentException ex)
            {
                throw new Exceptions.InvalidKeyException(ex);
            }
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
