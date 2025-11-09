// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Cryptography.Loader;
using NUnit.Framework.Legacy;

namespace MKW.Cryptography.Tests
{
    [Parallelizable]
#if !NETFRAMEWORK
    [TestFixture(SystemCryptographyLoader.Name)]
#endif
    [TestFixture(BouncyCastleLoader.Name)]
    public class CryptoTests
    {
        private readonly ICryptographyProvider crypto;

        public CryptoTests(string provider)
        {
            crypto = CryptographyLoader.GetProvider(provider);
        }

        [Test]
        public void DummySignTest()
        {
            AsymmetricPrivateKey key = crypto.CreateAsymmetricKey(CommonCryptographyAlgorithms.Rsa2048);
            using IAsymmetricPrivateTransformer transformer = crypto.OpenAsymmetricTransformer(
                key, CommonCryptographyAlgorithms.Rsa2048);

            ReadOnlyMemory<byte> data = EncodingConverter.GetBytes("killmepls");

            ReadOnlyMemory<byte> sign1 = transformer.Sign(data.Span);
            ReadOnlyMemory<byte> sign2 = transformer.Sign(data.Span);

            CollectionAssert.AreEqual(sign1.ToArray(), sign2.ToArray());

            ClassicAssert.IsTrue(transformer.Verify(data.Span, sign1.Span));
            ClassicAssert.IsFalse(transformer.Verify(data.Span, crypto.CreateRandomGenerator().NextBytes(sign1.Length)));
            ClassicAssert.IsFalse(transformer.Verify(data.Span, EncodingConverter.GetBytes("random123").Span));
        }

        [Test]
        public void UserCredentialsTests()
        {
            IUserCredentials pass1 = crypto.CreateUserCredentials("pass11",
                                                                  CommonCryptographyAlgorithms.Pbkdf2);

            IUserCredentials pass2 = crypto.OpenUserCredentials("pass11",
                                                                pass1.ExportSalt(),
                                                                CommonCryptographyAlgorithms.Pbkdf2);

            CollectionAssert.AreEqual(pass1.GetSecretKey().ToArray(), pass2.GetSecretKey().ToArray());
        }

        [Test]
        public void SymmetricTransformerTests()
        {
            SymmetricKey key = crypto.CreateSymmetricKey(CommonCryptographyAlgorithms.Aes128Gcm);
            using ISymmetricTransformer t1 = crypto.OpenSymmetricTransformer(key);

            byte[] data = [1, 2, 3];
            ReadOnlyMemory<byte> encrypted = t1.Encrypt(data);

            using ISymmetricTransformer t2 = crypto.OpenSymmetricTransformer(key);

            CollectionAssert.AreEqual(data, t1.Decrypt(encrypted.Span).ToArray());
            CollectionAssert.AreEqual(data, t2.Decrypt(encrypted.Span).ToArray());
        }

        [Test]
        [TestCase(0, 16)]
        [TestCase(1, 16)]
        [TestCase(100, 16)]
        [TestCase(127, 16)]
        [TestCase(128, 16)]
        [TestCase(129, 16)]
        [TestCase(255, 16)]
        [TestCase(256, 16)]
        [TestCase(257, 16)]
        [TestCase(1218, 16)]
        [TestCase(1024 * 1024, 2)] // 1 MB
        public void SymmetricTransformerRandomTests(int len, int extraTries)
        {
            SymmetricKey key = crypto.CreateSymmetricKey(CommonCryptographyAlgorithms.Aes128Gcm);
            using ISymmetricTransformer key1 = crypto.OpenSymmetricTransformer(key);

            byte[] data = new byte[len];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(i * 7213732 % 7892);
            }

            ReadOnlyMemory<byte> encrypted1 = key1.Encrypt(data);
            ReadOnlyMemory<byte> decrypted1 = key1.Decrypt(encrypted1.Span);

            CollectionAssert.AreEqual(data, decrypted1.ToArray());

            using ISymmetricTransformer key2 = crypto.OpenSymmetricTransformer(key);

            ReadOnlyMemory<byte> encrypted2 = key2.Encrypt(data);
            ReadOnlyMemory<byte> decrypted2 = key2.Decrypt(encrypted1.Span);

            CollectionAssert.AreEqual(encrypted1.ToArray(), encrypted2.ToArray());
            CollectionAssert.AreEqual(decrypted1.ToArray(), decrypted2.ToArray());

            for (int i = 0; i < extraTries; i++)
            {
                using ISymmetricTransformer key3 = crypto.OpenSymmetricTransformer(key);

                ReadOnlyMemory<byte> encrypted3 = key3.Encrypt(data);
                ReadOnlyMemory<byte> decrypted3 = key3.Decrypt(encrypted3.Span);

                CollectionAssert.AreEqual(encrypted1.ToArray(), encrypted3.ToArray());
                CollectionAssert.AreEqual(decrypted1.ToArray(), decrypted3.ToArray());
            }
        }

        [Test]
        public void AsymmetricTransformerTests()
        {
            SymmetricKeyAesGcm symkey = (SymmetricKeyAesGcm)crypto.CreateSymmetricKey(CommonCryptographyAlgorithms.Aes128Gcm);

            AsymmetricPrivateKey key = crypto.CreateAsymmetricKey(CommonCryptographyAlgorithms.Rsa2048);
            IAsymmetricPrivateTransformer transformer = crypto.OpenAsymmetricTransformer(
                key, CommonCryptographyAlgorithms.Rsa2048);

            ReadOnlyMemory<byte> data = symkey.KeyBytes;
            ReadOnlyMemory<byte> encrypted = transformer.Encrypt(data.Span);

            IAsymmetricPrivateTransformer decoder = crypto.OpenAsymmetricTransformer(
                key, CommonCryptographyAlgorithms.Rsa2048);

            CollectionAssert.AreEqual(data.ToArray(),
                                      transformer.Decrypt(encrypted.Span).ToArray());

            CollectionAssert.AreEqual(data.ToArray(),
                                      decoder.Decrypt(encrypted.Span).ToArray());

            CollectionAssert.AreNotEqual(transformer.Encrypt(data.Span).ToArray(),
                                         transformer.Encrypt(data.Span).ToArray());

            IAsymmetricPublicTransformer encoder = crypto.OpenAsymmetricTransformer(
                key.GetPublicKey(),
                CommonCryptographyAlgorithms.Rsa2048);

            ReadOnlyMemory<byte> encrypted2 = transformer.Encrypt(data.Span);
            CollectionAssert.AreNotEqual(encrypted.ToArray(),
                                         encrypted2.ToArray());

            CollectionAssert.AreEqual(data.ToArray(),
                                      decoder.Decrypt(encrypted2.Span).ToArray());
            CollectionAssert.AreEqual(data.ToArray(),
                                      transformer.Decrypt(encrypted2.Span).ToArray());
        }

        [Test]
        public void AsymmetricTransformerCreateBenchmark()
        {
            for (int i = 0; i < 10; i++)
            {
                crypto.CreateAsymmetricKey(CommonCryptographyAlgorithms.Rsa2048);
            }
        }

        [Test]
        [TestCase(5000)]
        public void AsymmetricTransformerOpenBenchmark(int iterations)
        {
            AsymmetricPrivateKey key = crypto.CreateAsymmetricKey(CommonCryptographyAlgorithms.Rsa2048);

            for (int i = 0; i < iterations; i++)
            {
                using IAsymmetricPublicTransformer t2 = crypto.OpenAsymmetricTransformer(
                    key.GetPublicKey(), CommonCryptographyAlgorithms.Rsa2048);
            }

            for (int i = 0; i < iterations; i++)
            {
                using IAsymmetricPrivateTransformer t2 = crypto.OpenAsymmetricTransformer(
                    key, CommonCryptographyAlgorithms.Rsa2048);
            }
        }
    }
}
