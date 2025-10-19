// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Core;
using MKW.Core.Serialization.OpenPgp;
using MKW.Cryptography;
using MKW.Cryptography.Loader;
using MKW.Storage.MKPG;
using NUnit.Framework.Legacy;
using Org.BouncyCastle.Bcpg;
using System.Buffers;

namespace MKW.Storage.Tests
{
    public class MKPGLowLevelTests
    {
        [Test]
        public void SimpleEntryEncode()
        {
            ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();

            DatabaseEntry dbEntry = new DatabaseEntry
            {
                Id = EntryId.Create(),
                Data = new byte[128],
                Salt = new byte[16],
                Keys = new Dictionary<UserId, ReadOnlyMemory<byte>>(),
            };

            EntrySerializer.Serialize(writer, dbEntry);

            using Stream stdout = Console.OpenStandardOutput();
            using ArmoredOutputStream armour = new ArmoredOutputStream(stdout);

            armour.Write(writer.WrittenSpan);
        }

        [Test]
        public void SimpleEntryDecode()
        {
            byte[] data = Convert.FromBase64String(
                "hQIMA87HrhcaeKn3AQ//TtsqaTQPGbNTPPuH0832+xVobQ8HYMSJeo3jXRh8Umjo49ZE85tmG93x" +
                "jnGrGzIygEQqUIF543hbxT1G4mXRU1gLmx0v4d0B5pH/3azKecB6PiRo4CBLRGJLhSQbRA45/9+C" +
                "+bjzE+v4NZ7qrXAN/PsRR+rkK+SUKqQxI3fifXI61/xiY7OkdrMb6xSnAnBNhs/U1+Iqxl6+nwjl" +
                "Dok0UXVS6meeWPS7FwS5i2LQkzCEuPcEPZxiH80uxuXGYKjYQnDzydniypukSs1P2ZXSJW59oL9W" +
                "jCYjjnLRL3zuL8mMa9wn+iWF7qSE7Wk9q8L84FV47etUbQw+1m5iVWVWg6QRLOaUE6Q5ja4GIK1S" +
                "fx5MLGokd82QDzFjvfu33m4GDoa+eq1T3OIe1izLsvtR6guuTruq1h/5GfkMei3O7iADQ/ZvU5Ww" +
                "CHqFM8I6SpCbsgoOgF2YOCMBaDKx0w6vyTzIZ/EFk3zMLi6aZvi1AxcrxzHxcgTcYjbuwAMB8gRb" +
                "mzb87rRt7l0PsXOn10m/iQYSi3HrHKulRRhNQVFOWAW51tXR2RcmtqBMwh1D1HeX5ox88LlNUQO3" +
                "Hw6+KqAAiayEcBX8kVAKu88JSeLTmDi22JxucqQ9iJAruzeNtGHcKYnn9X0K36UdiEQA+ipsgi4h" +
                "7gG2gFwfczFUgS9Mx37SdwEo2P/dRNKYAlOCKMFO71/y9LGCqS4Ep5t2HuB2SqZi6GAliadRxi9V" +
                "+6mamRhDv9PcDRc/NH2V6M4ip/m7eqpTwxGVrSotn4fI/ouVGgcNO447UpbHaiav7D1CETP5vykm" +
                "JoPWR/eBc2sju6nuENtcYFc+icy7");

            IBufferReader<byte> reader = new ArrayBufferReader<byte>(data);

            DatabaseEntry entry = EntrySerializer.Deserialize(reader, EntryId.Create());
        }

        [Test]
        public void EncodeDecodeUserTest()
        {
            ICryptographyProvider crypto = BouncyCastleLoader.GetProvider();
            IRandomGenerator random = crypto.CreateRandomGenerator();

            AsymmetricPrivateKey keypair = crypto.CreateAsymmetricKey(CommonCryptographyAlgorithms.Rsa2048);
            ReadOnlyMemory<byte> pubkey = crypto.EncodePkcsPublicKey(keypair.GetPublicKey());
            ReadOnlyMemory<byte> fakeseckey = random.NextBytes(432);

            DatabaseUser user = new DatabaseUser
            {
                Id = UserId.Create(),
                Salt = random.NextBytes(8),
                PrivateKey = new SecretPayload(fakeseckey),
                PublicKey = new SignedPayload(pubkey, new byte[32]),
                Metadata = null,
            };

            ArrayBufferWriter<byte> encoded = new ArrayBufferWriter<byte>();
            UserSerializer.Serialize(encoded, user);

            StringWriter writer = new StringWriter();
            PgpArmouredMessageSerializer.Serialize(writer, new PgpArmouredMessage
            {
                MessageTypeHeader = "MKW USER",
                Headers = [],
                Data = encoded.WrittenMemory,
            });
            Console.WriteLine(writer.ToString());

            ArrayBufferReader<byte> reader = new ArrayBufferReader<byte>(encoded.WrittenMemory);
            DatabaseUser decoded = UserSerializer.Deserialize(reader, user.Id);

            CollectionAssert.AreEqual(user.Salt.ToArray(), decoded.Salt.ToArray());

            CollectionAssert.AreEqual(user.PrivateKey.EncryptedPayload.ToArray(), decoded.PrivateKey.EncryptedPayload.ToArray());

            CollectionAssert.AreEqual(user.PublicKey.Payload.ToArray(), decoded.PublicKey.Payload.ToArray());
            CollectionAssert.AreEqual(user.PublicKey.Signature.ToArray(), decoded.PublicKey.Signature.ToArray());

            //CollectionAssert.AreEqual(user.AdminSignature.ToArray(), decoded.AdminSignature.ToArray());

            //CollectionAssert.AreEqual(user.Metadata.Payload.ToArray(), decoded.Metadata.Payload.ToArray());
            //CollectionAssert.AreEqual(user.Metadata.Signature.ToArray(), decoded.Metadata.Signature.ToArray());
        }
    }
}
