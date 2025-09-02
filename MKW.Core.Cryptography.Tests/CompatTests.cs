using MKW.Core.Cryptography.Loader;
using NUnit.Framework.Legacy;

namespace MKW.Core.Cryptography.Tests
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
            crypto1 = CryptographyLoader.Create(provider1);
            crypto2 = CryptographyLoader.Create(provider2);
        }

        [Test]
        public void UserCredentialsTests()
        {
            IUserCredentials pass1 = crypto1.CreateUserCredentials("pass11");
            IUserCredentials pass2 = crypto2.OpenUserCredentials("pass11", pass1.ExportSalt());

            CollectionAssert.AreEqual(pass1.GetSecretKey().ToArray(),
                                      pass2.GetSecretKey().ToArray());
        }

        [Test]
        public void SymmetricTransformerTests()
        {
            ISymmetricTransformer key1 = crypto1.CreateSymmetricTransformer();

            byte[] data = [1, 2, 3];
            Memory<byte> encrypted = key1.Encrypt(data);

            ISymmetricTransformer key2 = crypto2.OpenSymmetricTransformer(key1.ExportKey().Span,
                                                                          key1.ExportIV().Span);

            CollectionAssert.AreEqual(data, key2.Decrypt(encrypted.Span).ToArray());

            CollectionAssert.AreEqual(key1.Encrypt(data).ToArray(),
                                      key2.Encrypt(data).ToArray());
        }

        [Test]
        public void AsymmetricTransformerTests()
        {
            ISymmetricTransformer symkey = crypto1.CreateSymmetricTransformer();
            IAsymmetricPrivateTransformer key1 = crypto1.CreateAsymmetricTransformer();

            Memory<byte> data = symkey.ExportKey();

            IAsymmetricPrivateTransformer key2 = crypto2.OpenAsymmetricTransformer(key1.ExportPublicKey().Span,
                                                                                   key1.ExportPrivateKey().Span);

            CollectionAssert.AreEqual(data.ToArray(),
                                      key2.Decrypt(key1.Encrypt(data.Span).Span).ToArray());

            CollectionAssert.AreEqual(data.ToArray(),
                                      key1.Decrypt(key2.Encrypt(data.Span).Span).ToArray());
        }

        [Test]
        public void AsymmetricTransformerPublicKeyEncodeTests()
        {
            ISymmetricTransformer symkey = crypto1.CreateSymmetricTransformer();
            IAsymmetricPrivateTransformer decoder = crypto1.CreateAsymmetricTransformer();

            IAsymmetricPublicTransformer encoder = crypto2.OpenAsymmetricTransformer(decoder.ExportPublicKey().Span);

            Memory<byte> data = symkey.ExportKey();
            Memory<byte> encrypted = encoder.Encrypt(data.Span);

            CollectionAssert.AreEqual(data.ToArray(),
                                      decoder.Decrypt(encrypted.Span).ToArray());
        }

        [Test]
        public void AsymmetricTransformerSignTests()
        {
            using IAsymmetricPrivateTransformer singer1 = crypto1.CreateAsymmetricTransformer();
            using IAsymmetricPrivateTransformer singer2 = crypto2.OpenAsymmetricTransformer(
                singer1.ExportPublicKey().Span, singer1.ExportPrivateKey().Span);

            using IAsymmetricPublicTransformer verifier1 = crypto1.OpenAsymmetricTransformer(singer1.ExportPublicKey().Span);
            using IAsymmetricPublicTransformer verifier2 = crypto2.OpenAsymmetricTransformer(singer1.ExportPublicKey().Span);

            Memory<byte> data = EncodingConverter.GetBytes("data");

            Memory<byte> sign1 = singer1.Sign(data.Span);
            Memory<byte> sign2 = singer2.Sign(data.Span);

            CollectionAssert.AreEqual(sign1.ToArray(), sign2.ToArray());

            ClassicAssert.IsTrue(singer1.Verify(data.Span, sign2.Span));
            ClassicAssert.IsTrue(singer2.Verify(data.Span, sign1.Span));
            ClassicAssert.IsTrue(verifier1.Verify(data.Span, sign2.Span));
            ClassicAssert.IsTrue(verifier2.Verify(data.Span, sign1.Span));

            IRandomGenerator random = crypto1.CreateRandomGenerator();
            ClassicAssert.IsFalse(singer1.Verify(data.Span, random.NextBytes(sign1.Length)));
            ClassicAssert.IsFalse(singer1.Verify(data.Span, EncodingConverter.GetBytes("random123").Span));
            ClassicAssert.IsFalse(verifier1.Verify(data.Span, random.NextBytes(sign1.Length)));
            ClassicAssert.IsFalse(verifier1.Verify(data.Span, EncodingConverter.GetBytes("random123").Span));
        }
    }
}
