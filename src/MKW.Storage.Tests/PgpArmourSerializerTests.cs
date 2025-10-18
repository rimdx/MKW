// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Serialization.OpenPgp;
using NUnit.Framework.Legacy;

namespace MKW.Storage.Tests
{
    public class PgpArmourSerializerTests
    {
        [Test]
        public void SimpleSerializeTest()
        {
            using StringWriter writer = new StringWriter();

            PgpArmouredMessage msg = new PgpArmouredMessage
            {
                MessageTypeHeader = "PGP MESSAGE",
                Data = new byte[] { 1, 2, 3, 4 },
                Headers = [
                    new PgpArmourHeader("Version", "0.42.0"),
                ],
            };

            PgpArmouredMessageSerializer.Serialize(writer, msg);

            Console.WriteLine(writer.ToString());

            using StringReader reader = new StringReader(writer.ToString());

            PgpArmouredMessage? deserialized = PgpArmouredMessageSerializer.Deserialize(reader);

            ClassicAssert.NotNull(deserialized);
            ClassicAssert.AreEqual(msg.MessageTypeHeader, deserialized.MessageTypeHeader);
            CollectionAssert.AreEqual(msg.Data.ToArray(), deserialized.Data.ToArray());
        }

        [Test]
        [TestCase(PgpTestKeys.TestPublicKey)]
        [TestCase(PgpTestKeys.TestPrivateKey)]
        [TestCase(PgpTestKeys.PublicKeyEncryptedMessage)]
        [TestCase(PgpTestKeys.SymmetricallyEncryptedMessage)]
        public void Samples(string content)
        {
            using StringReader reader = new StringReader(content);

            PgpArmouredMessage? deserialized = PgpArmouredMessageSerializer.Deserialize(reader);
            ClassicAssert.NotNull(deserialized);

            using StringWriter writer = new StringWriter();
            writer.NewLine = "\n";

            PgpArmouredMessageSerializer.Serialize(writer, deserialized);

            Console.WriteLine(writer.ToString());

            ClassicAssert.AreEqual(content, writer.ToString());
        }

        [Test]
        public void BadChecksumTest()
        {
            using StringReader reader = new StringReader(
                "-----BEGIN PGP MESSAGE-----\n" +
                "\n" +
                "jA0ECQMC0V4B4tm8eOv/0nMBOjcOX9A5lnnQqbKjU3XLdd2inCXVQDl4mZQAvxOY\n" +
                "19BN6KueKQE3PWF1BxSRW8j5ZTIP4+8z3dh/vFb4jKk2pFTpc/IkgV7XczIePtgm\n" +
                "GBsdLUbQ8/GRRNwGu5jDU6sGlJ95/ltjHoa+bWV2Ta/EkzfP\n" +
                "=9aJA\n" +
                "-----END PGP MESSAGE-----");

            // $ gpg -d *asc
            // gpg: CRC error; F5A254 - F5A240

            // MKW.Core.Serialization.Pgp.Crc24ChecksumMismatchException : CRC checksum mismatch: F5A254 - F5A240

            Assert.Throws<Crc24ChecksumMismatchException>(
                () => PgpArmouredMessageSerializer.Deserialize(reader),
                "CRC checksum mismatch: F5A240 - F5A254"
            );
        }
    }
}
