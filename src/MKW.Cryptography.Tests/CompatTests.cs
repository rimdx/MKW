using MKW.Cryptography.Loader;
using NUnit.Framework.Legacy;

namespace MKW.Cryptography.Tests
{
    [Parallelizable]
#if NETFRAMEWORK
    [TestFixture(BouncyCastleLoader.Name, BouncyCastleLoader.Name)]
#else
    // Compat between different modules
    [TestFixture(SystemCryptographyLoader.Name, BouncyCastleLoader.Name)]
    [TestFixture(BouncyCastleLoader.Name, SystemCryptographyLoader.Name)]
    // Self compat
    [TestFixture(BouncyCastleLoader.Name, BouncyCastleLoader.Name)]
    [TestFixture(SystemCryptographyLoader.Name, SystemCryptographyLoader.Name)]
#endif
    public class CompatTests
    {
        private readonly ICryptographyProvider crypto1;
        private readonly ICryptographyProvider crypto2;

        public CompatTests(string provider1, string provider2)
        {
            crypto1 = CryptographyLoader.GetProvider(provider1);
            crypto2 = CryptographyLoader.GetProvider(provider2);
        }

        [Test]
        public void UserCredentialsTests()
        {
            IUserCredentials pass1 = crypto1.CreateUserCredentials("pass11",
                                                                   CommonCryptographyAlgorithms.Pbkdf2);

            IUserCredentials pass2 = crypto2.OpenUserCredentials("pass11",
                                                                 pass1.ExportSalt(),
                                                                 CommonCryptographyAlgorithms.Pbkdf2);

            CollectionAssert.AreEqual(pass1.GetSecretKey().ToArray(),
                                      pass2.GetSecretKey().ToArray());
        }

        [Test]
        public void SymmetricTransformerTests()
        {
            SymmetricKey key = crypto1.CreateSymmetricKey(CommonCryptographyAlgorithms.Aes128Gcm);

            using ISymmetricTransformer key1 = crypto1.OpenSymmetricTransformer(key);

            byte[] data = [1, 2, 3];
            Memory<byte> encrypted = key1.Encrypt(data);

            using ISymmetricTransformer key2 = crypto2.OpenSymmetricTransformer(key);

            CollectionAssert.AreEqual(encrypted.ToArray(),
                                      key2.Encrypt(data).ToArray());

            CollectionAssert.AreEqual(data, key2.Decrypt(encrypted.Span).ToArray());
        }

        [Test]
        public void AsymmetricTransformerTests()
        {
            AsymmetricPrivateKey key = crypto1.CreateAsymmetricKey(CommonCryptographyAlgorithms.Rsa2048);
            SymmetricKey symkey = crypto1.CreateSymmetricKey(CommonCryptographyAlgorithms.Aes128Gcm);

            IAsymmetricPrivateTransformer key1 = crypto1.OpenAsymmetricTransformer(
                key, CommonCryptographyAlgorithms.Rsa2048);

            ReadOnlyMemory<byte> data = symkey.KeyBytes;

            ReadOnlyMemory<byte> keyEncoded = crypto1.EncodePkcsPrivateKey(key);
            AsymmetricPrivateKey keyDecoded = crypto2.DecodePkcsPrivateKey(keyEncoded.Span);

            IAsymmetricPrivateTransformer key2 = crypto2.OpenAsymmetricTransformer(
                keyDecoded, CommonCryptographyAlgorithms.Rsa2048);

            CollectionAssert.AreEqual(data.ToArray(),
                                      key2.Decrypt(key1.Encrypt(data.Span).Span).ToArray());

            CollectionAssert.AreEqual(data.ToArray(),
                                      key1.Decrypt(key2.Encrypt(data.Span).Span).ToArray());
        }

        [Test]
        public void AsymmetricTransformerPublicKeyEncodeTests()
        {
            SymmetricKey symkey = crypto1.CreateSymmetricKey(CommonCryptographyAlgorithms.Aes128Gcm);
            AsymmetricPrivateKey key = crypto1.CreateAsymmetricKey(CommonCryptographyAlgorithms.Rsa2048);

            IAsymmetricPrivateTransformer decoder = crypto1.OpenAsymmetricTransformer(
                key, CommonCryptographyAlgorithms.Rsa2048);

            IAsymmetricPublicTransformer encoder = crypto2.OpenAsymmetricTransformer(
                key.GetPublicKey(), CommonCryptographyAlgorithms.Rsa2048);

            ReadOnlyMemory<byte> data = symkey.KeyBytes;
            Memory<byte> encrypted = encoder.Encrypt(data.Span);

            CollectionAssert.AreEqual(data.ToArray(),
                                      decoder.Decrypt(encrypted.Span).ToArray());
        }

        [Test]
        public void AsymmetricTransformerSignTests()
        {
            AsymmetricPrivateKey key = crypto1.CreateAsymmetricKey(CommonCryptographyAlgorithms.Rsa2048);

            using IAsymmetricPrivateTransformer signer1 = crypto1.OpenAsymmetricTransformer(
                key, CommonCryptographyAlgorithms.Rsa2048);
            using IAsymmetricPrivateTransformer signer2 = crypto2.OpenAsymmetricTransformer(
                key, CommonCryptographyAlgorithms.Rsa2048);

            using IAsymmetricPublicTransformer verifier1 = crypto1.OpenAsymmetricTransformer(
                key.GetPublicKey(), CommonCryptographyAlgorithms.Rsa2048);
            using IAsymmetricPublicTransformer verifier2 = crypto2.OpenAsymmetricTransformer(
                key.GetPublicKey(), CommonCryptographyAlgorithms.Rsa2048);

            Memory<byte> data = EncodingConverter.GetBytes("data");

            Memory<byte> sign1 = signer1.Sign(data.Span);
            Memory<byte> sign2 = signer2.Sign(data.Span);

            CollectionAssert.AreEqual(sign1.ToArray(), sign2.ToArray());

            ClassicAssert.IsTrue(signer1.Verify(data.Span, sign2.Span));
            ClassicAssert.IsTrue(signer2.Verify(data.Span, sign1.Span));
            ClassicAssert.IsTrue(verifier1.Verify(data.Span, sign2.Span));
            ClassicAssert.IsTrue(verifier2.Verify(data.Span, sign1.Span));

            IRandomGenerator random = crypto1.CreateRandomGenerator();
            ClassicAssert.IsFalse(signer1.Verify(data.Span, random.NextBytes(sign1.Length)));
            ClassicAssert.IsFalse(signer1.Verify(data.Span, EncodingConverter.GetBytes("random123").Span));
            ClassicAssert.IsFalse(verifier1.Verify(data.Span, random.NextBytes(sign1.Length)));
            ClassicAssert.IsFalse(verifier1.Verify(data.Span, EncodingConverter.GetBytes("random123").Span));
        }

        [Test]
        [Repeat(50)]
        public void PassAsymmetricKeysCrashTest()
        {
            AsymmetricPrivateKey key = crypto1.CreateAsymmetricKey(CommonCryptographyAlgorithms.Rsa2048);

            using IAsymmetricPrivateTransformer key2 = crypto2.OpenAsymmetricTransformer(
                key, CommonCryptographyAlgorithms.Rsa2048);

            using IAsymmetricPublicTransformer pubkey2 = crypto2.OpenAsymmetricTransformer(
                key.GetPublicKey(), CommonCryptographyAlgorithms.Rsa2048);
        }
    }
}
