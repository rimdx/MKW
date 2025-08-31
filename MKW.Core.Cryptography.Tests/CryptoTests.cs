using MKW.Core.Cryptography;
using MKW.Core.Cryptography.System;
using NUnit.Framework.Legacy;
using System.Security.Cryptography;

namespace MKW.Tests
{
    [TestFixture(typeof(CryptographyProvider))]
    public class CryptoTests<TProvider> where TProvider : ICryptographyProvider, new()
    {
        private readonly ICryptographyProvider crypto;

        public CryptoTests()
        {
            crypto = new TProvider();
        }

        [Test]
        public void DummySignTest()
        {
            using IAsymmetricPrivateTransformer transformer = crypto.CreateAsymmetricTransformer();

            Memory<byte> data = EncodingConverter.GetBytes("killmepls");

            Memory<byte> sign1 = transformer.Sign(data.Span);
            Memory<byte> sign2 = transformer.Sign(data.Span);

            CollectionAssert.AreEqual(sign1.ToArray(), sign2.ToArray());

            ClassicAssert.IsTrue(transformer.Verify(data.Span, sign1.Span));
            ClassicAssert.IsFalse(transformer.Verify(data.Span, RandomNumberGenerator.GetBytes(sign1.Length)));
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
            ISymmetricTransformer key1 = crypto.CreateSymmetricTransformer();

            byte[] data = [1, 2, 3];
            Memory<byte> encrypted = key1.Encrypt(data);

            ISymmetricTransformer key2 = crypto.OpenSymmetricTransformer(key1.ExportKey().Span, key1.ExportIV().Span);

            CollectionAssert.AreEqual(data, key1.Decrypt(encrypted.Span).ToArray());
            CollectionAssert.AreEqual(data, key2.Decrypt(encrypted.Span).ToArray());
        }

        [Test]
        public void AsymmetricTransformerTests()
        {
            ISymmetricTransformer symkey = crypto.CreateSymmetricTransformer();
            IAsymmetricPrivateTransformer key = crypto.CreateAsymmetricTransformer();

            Memory<byte> data = symkey.ExportKey();
            Memory<byte> encrypted = key.Encrypt(data.Span);

            IAsymmetricPrivateTransformer decoder = crypto.OpenAsymmetricTransformer(key.ExportPublicKey().Span, key.ExportPrivateKey().Span);

            CollectionAssert.AreEqual(data.ToArray(),
                                      key.Decrypt(encrypted.Span).ToArray());

            CollectionAssert.AreEqual(data.ToArray(),
                                      decoder.Decrypt(encrypted.Span).ToArray());

            CollectionAssert.AreNotEqual(key.Encrypt(data.Span).ToArray(),
                                         key.Encrypt(data.Span).ToArray());

            IAsymmetricPublicTransformer encoder = crypto.OpenAsymmetricTransformer(key.ExportPublicKey().Span);

            Memory<byte> encrypted2 = key.Encrypt(data.Span);
            CollectionAssert.AreNotEqual(encrypted.ToArray(),
                                         encrypted2.ToArray());

            CollectionAssert.AreEqual(data.ToArray(),
                                      decoder.Decrypt(encrypted2.Span).ToArray());
            CollectionAssert.AreEqual(data.ToArray(),
                                      key.Decrypt(encrypted2.Span).ToArray());
        }
    }
}
