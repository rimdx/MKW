// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Cryptography.Loader;
using NUnit.Framework.Legacy;

namespace MKW.Cryptography.Tests
{
    public class OpenPgpTests
    {
        [Test]
        public void SimpleAesTest()
        {
            ICryptographyProvider crypto = BouncyCastleLoader.GetProvider();
            IRandomGenerator random = crypto.CreateRandomGenerator();

            SymmetricKey key = new SymmetricKey
            {
                Engine = SymmetricAlgorithmEngine.AesOpenPgpCfb,
                KeyBytes = random.NextBytes(16),
                IVBytes = new byte[16],
            };

            using ISymmetricTransformer transformer = crypto.OpenSymmetricTransformer(key);

            ReadOnlyMemory<byte> data = random.NextBytes(42);

            ReadOnlyMemory<byte> enc = transformer.Encrypt(data.Span);
            ReadOnlyMemory<byte> dec = transformer.Decrypt(enc.Span);

            CollectionAssert.AreEqual(data.ToArray(), dec.ToArray());
        }

        [Test]
        public void SimpleAesPasswordDerived()
        {
            ICryptographyProvider crypto = BouncyCastleLoader.GetProvider();
            IRandomGenerator random = crypto.CreateRandomGenerator();
            IUserCredentials creds = crypto.CreateUserCredentials("123", CommonCryptographyAlgorithms.OpenPgpStringToKey);

            SymmetricKey key = new SymmetricKey
            {
                Engine = SymmetricAlgorithmEngine.AesOpenPgpCfb,
                KeyBytes = creds.GetSecretKey(),
                IVBytes = new byte[16],
            };

            using ISymmetricTransformer transformer = crypto.OpenSymmetricTransformer(key);

            ReadOnlyMemory<byte> data = random.NextBytes(42);

            ReadOnlyMemory<byte> enc = transformer.Encrypt(data.Span);
            ReadOnlyMemory<byte> dec = transformer.Decrypt(enc.Span);

            CollectionAssert.AreEqual(data.ToArray(), dec.ToArray());
        }
    }
}
