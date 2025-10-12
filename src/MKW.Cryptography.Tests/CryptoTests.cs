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
            using IAsymmetricPrivateTransformer transformer = crypto.CreateAsymmetricTransformer(CommonCryptographyAlgorithms.Rsa2048);

            Memory<byte> data = EncodingConverter.GetBytes("killmepls");

            Memory<byte> sign1 = transformer.Sign(data.Span);
            Memory<byte> sign2 = transformer.Sign(data.Span);

            CollectionAssert.AreEqual(sign1.ToArray(), sign2.ToArray());

            ClassicAssert.IsTrue(transformer.Verify(data.Span, sign1.Span));
            ClassicAssert.IsFalse(transformer.Verify(data.Span, crypto.CreateRandomGenerator().NextBytes(sign1.Length)));
            ClassicAssert.IsFalse(transformer.Verify(data.Span, EncodingConverter.GetBytes("random123").Span));
        }

        [Test]
        public void UserCredentialsTests()
        {
            IUserCredentials pass1 = crypto.CreateUserCredentials("pass11");
            IUserCredentials pass2 = crypto.OpenUserCredentials("pass11", pass1.ExportSalt());

            CollectionAssert.AreEqual(pass1.GetSecretKey().ToArray(), pass2.GetSecretKey().ToArray());
        }

        [Test]
        public void SymmetricTransformerTests()
        {
            ISymmetricTransformer key1 = crypto.CreateSymmetricTransformer(CommonCryptographyAlgorithms.Aes128Gcm);

            byte[] data = [1, 2, 3];
            Memory<byte> encrypted = key1.Encrypt(data);

            ISymmetricTransformer key2 = crypto.OpenSymmetricTransformer(
                key1.ExportKey().Span,
                key1.ExportIV().Span,
                CommonCryptographyAlgorithms.Aes128Gcm);

            CollectionAssert.AreEqual(data, key1.Decrypt(encrypted.Span).ToArray());
            CollectionAssert.AreEqual(data, key2.Decrypt(encrypted.Span).ToArray());
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
            ISymmetricTransformer key1 = crypto.CreateSymmetricTransformer(CommonCryptographyAlgorithms.Aes128Gcm);

            byte[] data = new byte[len];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(i * 7213732 % 7892);
            }

            Memory<byte> encrypted1 = key1.Encrypt(data);
            Memory<byte> decrypted1 = key1.Decrypt(encrypted1.Span);

            CollectionAssert.AreEqual(data, decrypted1.ToArray());

            ISymmetricTransformer key2 = crypto.OpenSymmetricTransformer(
                key1.ExportKey().Span, key1.ExportIV().Span, CommonCryptographyAlgorithms.Aes128Gcm);

            Memory<byte> encrypted2 = key2.Encrypt(data);
            Memory<byte> decrypted2 = key2.Decrypt(encrypted1.Span);

            CollectionAssert.AreEqual(encrypted1.ToArray(), encrypted2.ToArray());
            CollectionAssert.AreEqual(decrypted1.ToArray(), decrypted2.ToArray());

            for (int i = 0; i < extraTries; i++)
            {
                ISymmetricTransformer key3 = crypto.OpenSymmetricTransformer(
                    key1.ExportKey().Span,
                    key1.ExportIV().Span,
                    CommonCryptographyAlgorithms.Aes128Gcm);

                Memory<byte> encrypted3 = key3.Encrypt(data);
                Memory<byte> decrypted3 = key3.Decrypt(encrypted3.Span);

                CollectionAssert.AreEqual(encrypted1.ToArray(), encrypted3.ToArray());
                CollectionAssert.AreEqual(decrypted1.ToArray(), decrypted3.ToArray());
            }
        }

        [Test]
        public void AsymmetricTransformerTests()
        {
            ISymmetricTransformer symkey = crypto.CreateSymmetricTransformer(CommonCryptographyAlgorithms.Aes128Gcm);
            IAsymmetricPrivateTransformer key = crypto.CreateAsymmetricTransformer(CommonCryptographyAlgorithms.Rsa2048);

            Memory<byte> data = symkey.ExportKey();
            Memory<byte> encrypted = key.Encrypt(data.Span);

            IAsymmetricPrivateTransformer decoder = crypto.OpenAsymmetricTransformer(
                key.ExportPublicKey().Span,
                key.ExportPrivateKey().Span,
                CommonCryptographyAlgorithms.Rsa2048);

            CollectionAssert.AreEqual(data.ToArray(),
                                      key.Decrypt(encrypted.Span).ToArray());

            CollectionAssert.AreEqual(data.ToArray(),
                                      decoder.Decrypt(encrypted.Span).ToArray());

            CollectionAssert.AreNotEqual(key.Encrypt(data.Span).ToArray(),
                                         key.Encrypt(data.Span).ToArray());

            IAsymmetricPublicTransformer encoder = crypto.OpenAsymmetricTransformer(
                key.ExportPublicKey().Span,
                CommonCryptographyAlgorithms.Rsa2048);

            Memory<byte> encrypted2 = key.Encrypt(data.Span);
            CollectionAssert.AreNotEqual(encrypted.ToArray(),
                                         encrypted2.ToArray());

            CollectionAssert.AreEqual(data.ToArray(),
                                      decoder.Decrypt(encrypted2.Span).ToArray());
            CollectionAssert.AreEqual(data.ToArray(),
                                      key.Decrypt(encrypted2.Span).ToArray());
        }

        [Test]
        public void AsymmetricTransformerCreateBenchmark()
        {
            for (int i = 0; i < 10; i++)
            {
                using IAsymmetricPrivateTransformer transformer = crypto.CreateAsymmetricTransformer(CommonCryptographyAlgorithms.Rsa2048);
                _ = transformer.ExportPrivateKey();
                _ = transformer.ExportPublicKey();
            }
        }

        [Test]
        [TestCase(5000)]
        public void AsymmetricTransformerOpenBenchmark(int iterations)
        {
            using IAsymmetricPrivateTransformer transformer = crypto.CreateAsymmetricTransformer(CommonCryptographyAlgorithms.Rsa2048);

            Memory<byte> priv = transformer.ExportPrivateKey();
            Memory<byte> pub = transformer.ExportPublicKey();

            for (int i = 0; i < iterations; i++)
            {
                using IAsymmetricPublicTransformer t2 = crypto.OpenAsymmetricTransformer(
                    pub.Span,
                    CommonCryptographyAlgorithms.Rsa2048);
            }

            for (int i = 0; i < iterations; i++)
            {
                using IAsymmetricPrivateTransformer t2 = crypto.OpenAsymmetricTransformer(
                    pub.Span,
                    priv.Span,
                    CommonCryptographyAlgorithms.Rsa2048);
            }
        }
    }
}
