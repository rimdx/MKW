using NUnit.Framework.Legacy;
using System.Collections;
using System.Text;

namespace MKW.Core.Common.Tests
{
    [TestFixture]
    [Parallelizable]
    public class Base32Tests
    {
        [Test]
        [TestCase(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 }, "AAAAAAAA")]
        [TestCase(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x01 }, "AAAAAAAB")]
        [TestCase(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x0F }, "AAAAAAAP")]
        [TestCase(new byte[] { 0x0F, 0x00, 0x00, 0x00, 0x00 }, "B4AAAAAA")]
        [TestCase(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 }, "AEBAGBAF")]
        public void SimpleOneChunk(byte[] data, string base32)
        {
            BitArray bits = new BitArray(Base32BitConvert.Base32NumberWidth * Base32BitConvert.BufferNumberWidth);
            byte[] output = new byte[Base32BitConvert.OutputChunkSize];

            Base32BitConvert.BufferToBits(data, bits);
            Base32BitConvert.EncodeChunk(bits, bits.Length, output);

            ClassicAssert.AreEqual(base32, Encoding.UTF8.GetString(output));
        }

        [Test]
        public void OneChunkPadding()
        {
            BitArray bits = new BitArray(Base32BitConvert.Base32NumberWidth * Base32BitConvert.BufferNumberWidth);

            byte[] input = [0xFF, 0x23];
            byte[] output = new byte[Base32BitConvert.OutputChunkSize];

            Base32BitConvert.BufferToBits(input, bits);
            int count = Base32BitConvert.EncodeChunk(bits, 16, output);

            ClassicAssert.AreEqual(4, count);
        }

        [Test]
        [TestCase("The quick brown fox jumps over the lazy dog.", "KRUGKIDROVUWG2ZAMJZG653OEBTG66BANJ2W24DTEBXXMZLSEB2GQZJANRQXU6JAMRXWOLQ=")]
        [TestCase("qq", "OFYQ====")]
        [TestCase("1", "GE======")]
        [TestCase("12345", "GEZDGNBV")]
        public void TextTransform(string text, string base32)
        {
            ClassicAssert.AreEqual(base32, Base32Convert.Encode(Encoding.UTF8.GetBytes(text)));
        }

        [Test]
        public void BufferFrom0To255()
        {
            byte[] buffer = new byte[256];

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)i;

                string chars = "0123456789ABCDEF";
                Console.Write(chars[(i & 0xF0) >> 4]);
                Console.Write(chars[(i & 0x0F) >> 0]);
            }

            ClassicAssert.AreEqual(
                "AAAQEAYEAUDAOCAJBIFQYDIOB4IBCEQTCQKRMFYYDENBWHA5DYPSAIJCEMSCKJRHFAUSUKZMFUXC6MBRGIZTINJWG44DSOR3HQ6T4P2AIFBEGRCFIZDUQSKKJNGE2TSPKBIVEU2UKVLFOWCZLJNVYXK6L5QGCYTDMRSWMZ3INFVGW3DNNZXXA4LSON2HK5TXPB4XU634PV7H7AEBQKBYJBMGQ6EITCULRSGY5D4QSGJJHFEVS2LZRGM2TOOJ3HU7UCQ2FI5EUWTKPKFJVKV2ZLNOV6YLDMVTWS23NN5YXG5LXPF5X274BQOCYPCMLRWHZDE4VS6MZXHM7UGR2LJ5JVOW27MNTWW33TO55X7A4HROHZHF43T6R2PK5PWO33XP6DY7F47U6X3PP6HZ7L57Z7P674======",
                Base32Convert.Encode(buffer));
        }

        [Test]
        [TestCase(4 * 1024)] // 4 KB
        [TestCase(16 * 1024 * 1024)] // 16 MB
        public void LargeBufferRandomTest(int length)
        {
            byte[] buffer = new byte[length];

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(i % 700 % 256); // pseudo random
            }

            Base32Convert.Encode(buffer);
        }
    }
}
