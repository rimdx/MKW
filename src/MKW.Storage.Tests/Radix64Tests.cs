using MKW.Core.Serialization.Pgp;
using MKW.Common;
using Org.BouncyCastle.Bcpg;
using System.Security.Cryptography;
using NUnit.Framework.Legacy;
using Microsoft.VisualBasic;
using System.Text;

namespace MKW.Storage.Tests
{
    public class Radix64Tests
    {
        [Test]
        public void EncodeTest()
        {
            using Radix64Encoder transformer = new Radix64Encoder();
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
    }
}
