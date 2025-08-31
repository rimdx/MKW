using MKW.Core.Cryptography;
using MKW.Core.Cryptography.System;
using NUnit.Framework.Legacy;
using System.Security.Cryptography;

namespace MKW.Tests
{
    public class CryptoTests
    {
        [Test]
        public void DummySignTest()
        {
            using IAsymmetricPrivateTransformer transformer = AsymmetricTransformer.Create();

            Memory<byte> data = EncodingConverter.GetBytes("killmepls");

            Memory<byte> sign1 = transformer.Sign(data.Span);
            Memory<byte> sign2 = transformer.Sign(data.Span);

            CollectionAssert.AreEqual(sign1.ToArray(), sign2.ToArray());

            ClassicAssert.IsTrue(transformer.Verify(data.Span, sign1.Span));
            ClassicAssert.IsFalse(transformer.Verify(data.Span, RandomNumberGenerator.GetBytes(sign1.Length)));
            ClassicAssert.IsFalse(transformer.Verify(data.Span, EncodingConverter.GetBytes("random123").Span));
        }
    }
}
