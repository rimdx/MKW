// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Serialization.OpenPgp;
using MKW.Cryptography;
using MKW.Cryptography.Loader;
using MKW.Testing.Common;
using NUnit.Framework.Legacy;
using System.Text;

namespace MKW.OpenPgp.Tests
{
    [TestFixture]
    public class PrivateKeyTests
    {
        private static readonly ICryptographyProvider crypto = BouncyCastleLoader.GetProvider();

        [Test]
        public void SimpleTest()
        {
            PgpArmouredMessage? keyArmour = PgpArmouredMessageSerializer.Deserialize(new StreamReader(PgpTestKeys.TestPrivateKey));
            ClassicAssert.NotNull(keyArmour);

            PgpArmouredMessage? msgArmour = PgpArmouredMessageSerializer.Deserialize(new StreamReader(PgpTestKeys.PublicKeyEncryptedMessageAes128NoCompression));
            ClassicAssert.NotNull(msgArmour);

            PgpPrivateKey key = new PgpPrivateKey(crypto);
            key.Import(keyArmour);

            PgpMessage msg = PgpMessage.Open(msgArmour);

            ReadOnlyMemory<byte> plaintext = key.DecryptMessage(msg);

            Console.WriteLine(Encoding.UTF8.GetString(plaintext.ToArray()));
        }
    }
}
