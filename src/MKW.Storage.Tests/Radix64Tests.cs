// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Core.Serialization.Pgp;
using NUnit.Framework.Legacy;
using Org.BouncyCastle.Bcpg;
using System.Security.Cryptography;
using System.Text;

namespace MKW.Storage.Tests
{
    public class Radix64Tests
    {
        [Test]
        public void EncodeTest()
        {
            Crc24 crc = new Crc24();
            using Radix64Encoder transformer = new Radix64Encoder(crc);
            using MemoryStream output = new MemoryStream();
            using CryptoStream stream = new CryptoStream(output, transformer, CryptoStreamMode.Write);

            Span<byte> data = [0x14, 0xFB, 0x9C, 0x03, 0xD9, 0x7E];
            stream.Write(data);

            Span<byte> expected = [
                (byte)'F',
                (byte)'P',
                (byte)'u',
                (byte)'c',
                (byte)'A',
                (byte)'9',
                (byte)'l',
                (byte)'+',
            ];

            CollectionAssert.AreEqual(expected.ToArray(), output.ToArray());
            Console.Write(Encoding.ASCII.GetString(output.ToArray()));
        }

        [Test]
        public void DecodeTest()
        {
            Crc24 crc = new Crc24();
            using Radix64Decoder transformer = new Radix64Decoder(crc);
            using MemoryStream output = new MemoryStream();
            using CryptoStream stream = new CryptoStream(output, transformer, CryptoStreamMode.Write);

            Span<byte> encoded = [
                (byte)'F',
                (byte)'P',
                (byte)'u',
                (byte)'c',
                (byte)'A',
                (byte)'9',
                (byte)'l',
                (byte)'+',
            ];

            Span<byte> decoded = [0x14, 0xFB, 0x9C, 0x03, 0xD9, 0x7E];

            stream.Write(encoded);
            CollectionAssert.AreEqual(decoded.ToArray(), output.ToArray());
        }
    }
}
