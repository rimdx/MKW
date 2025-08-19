using MKW.Core.Cryptography;
using NUnit.Framework.Legacy;
using System.Security.Cryptography;

namespace MKW.Tests
{
    public class CryptoTests
    {
        [Test]
        public void DummySignTest()
        {
            using AsymmetricTransformer transformer = AsymmetricTransformer.Create();

            byte[] data = EncodingConverter.GetBytes("killmepls");

            byte[] sign1 = transformer.Sign(data);
            byte[] sign2 = transformer.Sign(data);

            CollectionAssert.AreEqual(sign1, sign2);

            ClassicAssert.IsTrue(transformer.Verify(data, sign1));
            ClassicAssert.IsFalse(transformer.Verify(data, RandomNumberGenerator.GetBytes(sign1.Length)));
            ClassicAssert.IsFalse(transformer.Verify(data, EncodingConverter.GetBytes("random123")));
        }
    }
}
