// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Core.Serialization.OpenPgp;
using MKW.Core.Serialization.OpenPgp.Packets;
using MKW.Cryptography;
using MKW.Cryptography.Loader;
using NUnit.Framework.Legacy;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Buffers;

namespace MKW.Storage.Tests
{
    public class PgpTests
    {
        [Test]
        public void PgpSerializerTests()
        {
            byte[] data = [140, 13, 4, 9, 3, 2, 193, 70, 156, 83, 104, 68, 56, 62, 255, 210, 113, 1, 85, 135, 252, 140, 252, 244, 190, 36, 250, 178, 173, 195, 190, 248, 244, 117, 34, 178, 3, 187, 119, 12, 214, 57, 32, 203, 146, 138, 218, 82, 172, 130, 159, 197, 57, 144, 77, 140, 153, 217, 24, 196, 96, 129, 30, 108, 112, 130, 227, 0, 170, 58, 38, 210, 162, 150, 108, 207, 87, 222, 243, 225, 75, 72, 176, 222, 23, 59, 164, 15, 91, 238, 146, 136, 226, 127, 27, 65, 23, 182, 27, 47, 15, 158, 255, 193, 16, 80, 10, 206, 224, 210, 91, 46, 131, 74, 157, 212, 52, 205, 115, 66, 148, 218, 177, 121, 253, 186, 216, 28, 183, 122];

            ArrayBufferReader reader = new ArrayBufferReader(data);

            {
                PgpPacket packet = PgpPacketSerializer.ReadPacket(reader);
                ClassicAssert.AreEqual(PacketTag.SymmetricKeyEncryptedSessionKey, packet.Tag);

                ArrayBufferReader bodyReader = new ArrayBufferReader(packet.EncodedBody);
                SymEncryptedSessionKeyV4 key = SymEncryptedSessionKeyV4Serializer.Deserialize(bodyReader);
            }

            {
                PgpPacket packet = PgpPacketSerializer.ReadPacket(reader);
                ClassicAssert.AreEqual(PacketTag.SymmetricEncryptedIntegrityProtected, packet.Tag);

                ArrayBufferReader bodyReader = new ArrayBufferReader(packet.EncodedBody);
                SymEncryptedProtectedDataV1 key = SymEncryptedProtectedDataV1Serializer.Deserialize(bodyReader);
            }
        }

        [Test]
        public void PgpDecryptTest()
        {
            // Uint8Array.fromHex("aa").join(", ")

            // -----BEGIN PGP MESSAGE-----
            // jA0ECQMCwUacU2hEOD7/0nEBVYf8jPz0viT6sq3Dvvj0dSKyA7t3DNY5IMuSitpS
            // rIKfxTmQTYyZ2RjEYIEebHCC4wCqOibSopZsz1fe8+FLSLDeFzukD1vukojifxtB
            // F7YbLw+e/8EQUArO4NJbLoNKndQ0zXNClNqxef262By3eg==
            // =3yMU
            // -----END PGP MESSAGE-----

            // 3.7.1.3.  Iterated and Salted S2K
            //   Octet  0:        0x03
            //   Octet  1:        hash algorithm
            //   Octets 2-9:      8-octet salt value
            //   Octet  10:       count, a one-octet, coded value

            ReadOnlyMemory<byte> sessionKeyPacket = new byte[] {
                // version
                0x04,

                // A one-octet number describing the symmetric algorithm used.
                0x09,
                // -- AES with 256-bit key

                // === begin of S2K subpacket

                // hash algorithm
                0x03,
                // -- Iterated and Salted S2K

                0x02,
                // -- SHA-1

                // 8-octet salt value
                2, 193, 70, 156, 83, 104, 68, 56, 62,

                // count, a one-octet, coded value
                0xff

                // #define EXPBIAS 6
                // count = ((Int32)16 + (c & 15)) << ((c >> 4) + EXPBIAS);
                // count = (16 + (0xff & 15)) << ((0xff >> 4) + 6)
                // count = 65011712
            };

            byte[] dataPacket = [85, 135, 252, 140, 252, 244, 190, 36, 250, 178, 173, 195, 190, 248, 244, 117, 34, 178, 3, 187, 119, 12, 214, 57, 32, 203, 146, 138, 218, 82, 172, 130, 159, 197, 57, 144, 77, 140, 153, 217, 24, 196, 96, 129, 30, 108, 112, 130, 227, 0, 170, 58, 38, 210, 162, 150, 108, 207, 87, 222, 243, 225, 75, 72, 176, 222, 23, 59, 164, 15, 91, 238, 146, 136, 226, 127, 27, 65, 23, 182, 27, 47, 15, 158, 255, 193, 16, 80, 10, 206, 224, 210, 91, 46, 131, 74, 157, 212, 52, 205, 115, 66, 148, 218, 177, 121, 253, 186, 216, 28, 183, 122];
            string password = "123";

            ICryptographyProvider crypto = CryptographyLoader.GetProvider(BouncyCastleLoader.Name);

            ReadOnlyMemory<byte> salt = sessionKeyPacket.Slice(5, 8);

            PasswordDerivationConfiguration s2kConfig = new PasswordDerivationConfiguration
            {
                Engine = PasswordDerivationEngine.Pbkdf2,
                HashEngine = HashAlgorithmEngine.Sha256,
                Iterations = 1024,
                KeySizeBits = 256,
                SaltSizeBits = salt.Length * 8,
            };

            IUserCredentials creds = crypto.CreateUserCredentials(password, s2kConfig);

            SymmetricKey key = new SymmetricKey
            {
                Engine = SymmetricAlgorithmEngine.AesOpenPgpCfb,
                IVBytes = new byte[16],
                KeyBytes = creds.GetSecretKey(),
            };

            using ISymmetricTransformer transformer = crypto.OpenSymmetricTransformer(key);

            Memory<byte> decrypted = transformer.Decrypt(dataPacket);

            Console.Write(EncodingConverter.GetString(decrypted.Span));
        }

        [Test]
        public void PublicKeyParseTest()
        {
            using StringReader armourReader = new StringReader(PgpTestKeys.TestPublicKey);
            PgpArmouredMessage? msg = PgpArmouredMessageSerializer.Deserialize(armourReader);

            ClassicAssert.NotNull(msg);

            ArrayBufferReader messageReader = new ArrayBufferReader(msg.Data);

            while (messageReader.RemainingBytes > 0)
            {
                PgpPacket packet = PgpPacketSerializer.ReadPacket(messageReader);
                ArrayBufferReader packetReader = new ArrayBufferReader(packet.EncodedBody);

                if (packet.Tag == PacketTag.PublicKey)
                {
                    PublicKeyPacketV4 decoded = PublicKeyPacketV4Serializer.Deserialize(packetReader);

                    ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();
                    PublicKeyPacketV4Serializer.Serialize(writer, decoded);
                    CollectionAssert.AreEqual(packet.EncodedBody.ToArray(), writer.WrittenSpan.ToArray());
                }
            }
        }

        [Test]
        public void SecretKeyParseTest()
        {
            using StringReader armourReader = new StringReader(PgpTestKeys.TestPrivateKeyEncrypted);
            PgpArmouredMessage? msg = PgpArmouredMessageSerializer.Deserialize(armourReader);

            ClassicAssert.NotNull(msg);

            ArrayBufferReader messageReader = new ArrayBufferReader(msg.Data);

            while (messageReader.RemainingBytes > 0)
            {
                PgpPacket packet = PgpPacketSerializer.ReadPacket(messageReader);
                ArrayBufferReader packetReader = new ArrayBufferReader(packet.EncodedBody);

                if (packet.Tag == PacketTag.SecretKey)
                {
                    Core.Serialization.OpenPgp.Packets.SecretKeyPacket decoded = SecretKeyPacketSerializer.Deserialize(packetReader);

                    ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();
                    SecretKeyPacketSerializer.Serialize(writer, decoded);
                    CollectionAssert.AreEqual(packet.EncodedBody.ToArray(), writer.WrittenSpan.ToArray());
                }
            }
        }
    }
}
