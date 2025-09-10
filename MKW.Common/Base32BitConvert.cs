using System.Collections;

namespace MKW.Common
{
    public static class Base32BitConvert
    {
        private const string CharMap = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        private const char Padding = '=';

        public const int Base32NumberWidth = 5; // 5 bits = log2(32)
        public const int BufferNumberWidth = 8; // 8 bits = 1 byte

        public const int InputChunkSize = 5; // 5 bytes
        public const int OutputChunkSize = 8; // 8 chars

        /// <summary>
        /// Converts a byte buffer to BitArray
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public static void BufferToBits(ReadOnlySpan<byte> input, BitArray output)
        {
            int i = 0;

            // copy data from input to output, expanding bytes to bits
            for (; i < input.Length; i++)
            {
                for (int j = 0; j < BufferNumberWidth; j++)
                {
                    int mask = 1 << BufferNumberWidth - 1 - j;
                    bool bit = (input[i] & mask) > 0;
                    output[i * BufferNumberWidth + j] = bit;
                }
            }

            // fill the rest with zeros
            for (int bit = i * BufferNumberWidth; bit < output.Length; bit++)
            {
                output[bit] = false;
            }
        }

        /// <summary>
        /// Converts first <c>count</c> bytes into base32 encoding.
        /// </summary>
        /// <param name="bits"></param>
        /// <param name="output"></param>
        /// <returns>The amount of base32 symbols.</returns>
        public static int EncodeChunk(BitArray bits, int bitCount, Span<byte> output)
        {
            int bitIndex = 0;
            int i = 0;

            for (; bitIndex < bitCount; i++)
            {
                int c = 0;

                for (int j = 0; j < Base32NumberWidth; j++, bitIndex++)
                {
                    int mask = 1 << Base32NumberWidth - 1 - j;
                    int bit = bits[bitIndex] ? 1 : 0;
                    c += bit * mask;
                }

                output[i] = (byte)CharMap[c];
            }

            return i;
        }

        public static void WritePadding(Span<byte> output)
        {
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = (byte)Padding;
            }
        }
    }
}
