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

        private const byte padding = (byte)'=';

        private static readonly byte[] encodingTable =
        [
            (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G',
            (byte)'H', (byte)'I', (byte)'J', (byte)'K', (byte)'L', (byte)'M', (byte)'N',
            (byte)'O', (byte)'P', (byte)'Q', (byte)'R', (byte)'S', (byte)'T', (byte)'U',
            (byte)'V', (byte)'W', (byte)'X', (byte)'Y', (byte)'Z',
            (byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e', (byte)'f', (byte)'g',
            (byte)'h', (byte)'i', (byte)'j', (byte)'k', (byte)'l', (byte)'m', (byte)'n',
            (byte)'o', (byte)'p', (byte)'q', (byte)'r', (byte)'s', (byte)'t', (byte)'u',
            (byte)'v',
            (byte)'w', (byte)'x', (byte)'y', (byte)'z',
            (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6',
            (byte)'7', (byte)'8', (byte)'9',
            (byte)'+', (byte)'/'
        ];

        private readonly static ReadOnlyMemory<byte> decodingTable = CreateDecodingTable();

        private static ReadOnlyMemory<byte> CreateDecodingTable()
        {
            byte[] result = new byte[128];
            result.AsSpan().Fill(0xFF);

            for (int i = 'A'; i <= 'Z'; i++)
            {
                result[i] = (byte)(i - 'A');
            }

            for (int i = 'a'; i <= 'z'; i++)
            {
                result[i] = (byte)(i - 'a' + 26);
            }

            for (int i = '0'; i <= '9'; i++)
            {
                result[i] = (byte)(i - '0' + 52);
            }

            result['+'] = 62;
            result['/'] = 63;

            return result;
        }

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
            encoded[0] = encodingTable[0x3f & (bytes[0] >> 2)];
            encoded[1] = encodingTable[0x3f & (bytes[0] << 4) | (bytes[1] >> 4)];
            encoded[2] = encodingTable[0x3f & (bytes[1] << 2) | (bytes[2] >> 6)];
            encoded[3] = encodingTable[0x3f & (bytes[2])];
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
                encoded[0] = encodingTable[0x3f & (bytes[0] >> 2)];
                encoded[1] = encodingTable[0x3f & (bytes[0] << 4)];
                encoded[2] = padding;
                encoded[3] = padding;
            }
            else if (bytes.Length == 2)
            {
                encoded[0] = encodingTable[0x3f & (bytes[0] >> 2)];
                encoded[1] = encodingTable[0x3f & (bytes[0] << 4) | (bytes[1] >> 4)];
                encoded[2] = encodingTable[0x3f & (bytes[1] << 2)];
                encoded[3] = padding;
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
                byte value = decodingTable.Span[inputSpan[i]];

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
