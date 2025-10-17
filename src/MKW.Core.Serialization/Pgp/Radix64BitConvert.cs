using System.Collections;

namespace MKW.Core.Serialization.Pgp
{
    // 6-bit -> 8-bit
    // lcm(6, 8) = 24 bit = 3 byte (of input)
    // 3 * 8 / 6          = 4 byte (of output)
    internal static class Radix64BitConvert
    {
        private const int numberWidth = 6;
        private const int byteWidth = 8;

        public static void BufferFromBits(BitArray input, Span<byte> output)
        {
            for (int i = 0; i < input.Length / 8; i++)
            {
                int octet = 0;

                for (int j = 0; j < byteWidth; j++)
                {
                    if (input[(i * byteWidth) + j])
                    {
                        int mask = 1 << (byteWidth - 1 - j);
                        octet += mask;
                    }
                }

                output[i] = (byte)octet;
            }
        }

        // bytes.count = 3
        // encoded.count = 4
        public static void EncodeFullBlock(ReadOnlySpan<byte> bytes, Span<byte> encoded)
        {
            encoded[0] = Radix64Charset.EncodingTable[0x3f & (bytes[0] >> 2)];
            encoded[1] = Radix64Charset.EncodingTable[0x3f & (bytes[0] << 4) | (bytes[1] >> 4)];
            encoded[2] = Radix64Charset.EncodingTable[0x3f & (bytes[1] << 2) | (bytes[2] >> 6)];
            encoded[3] = Radix64Charset.EncodingTable[0x3f & (bytes[2])];
        }

        // bytes.count ~ (0, 3)
        // encoded.count = 4
        public static void EncodeFinalBlock(ReadOnlySpan<byte> bytes, Span<byte> encoded)
        {
            if (bytes.Length == 0)
            {
                // no work needed
            }
            else if (bytes.Length == 1)
            {
                encoded[0] = Radix64Charset.EncodingTable[0x3f & (bytes[0] >> 2)];
                encoded[1] = Radix64Charset.EncodingTable[0x3f & (bytes[0] << 4)];
                encoded[2] = Radix64Charset.Padding;
                encoded[3] = Radix64Charset.Padding;
            }
            else if (bytes.Length == 2)
            {
                encoded[0] = Radix64Charset.EncodingTable[0x3f & (bytes[0] >> 2)];
                encoded[1] = Radix64Charset.EncodingTable[0x3f & (bytes[0] << 4) | (bytes[1] >> 4)];
                encoded[2] = Radix64Charset.EncodingTable[0x3f & (bytes[1] << 2)];
                encoded[3] = Radix64Charset.Padding;
            }
            else if (encoded.Length == 3)
            {
                EncodeFullBlock(bytes, encoded);
            }
        }

        public static int DecodeChunk(BitArray bits, ReadOnlySpan<byte> inputSpan)
        {
            int i = 0;

            for (; i < inputSpan.Length; i++)
            {
                byte value = Radix64Charset.DecodingTable.Span[inputSpan[i]];

                for (int j = 0; j < numberWidth; j++)
                {
                    int mask = 1 << (numberWidth - 1 - j);
                    bool bit = (value & mask) > 0;
                    bits[i * numberWidth + j] = bit;
                }
            }

            return i;
        }
    }
}
