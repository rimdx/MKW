namespace MKW.Core.Serialization.Pgp
{
    // 6-bit -> 8-bit
    // lcm(6, 8) = 24 bit = 3 byte (of input)
    // 3 * 8 / 6          = 4 byte (of output)
    internal static class Radix64BitConvert
    {
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

        // encoded.count = 4
        // bytes.count = 3
        public static int DecodeBlock(ReadOnlySpan<byte> encoded, Span<byte> bytes)
        {
            if (encoded[1] == Radix64Charset.Padding)
            {
                int b0 = Radix64Charset.DecodeChar(encoded[0]);
                int b1 = Radix64Charset.DecodeChar(encoded[1]);

                bytes[0] = (byte)((b0 << 2) | (b1 >> 4));

                return 1;
            }
            else if (encoded[2] == Radix64Charset.Padding)
            {
                int b0 = Radix64Charset.DecodeChar(encoded[0]);
                int b1 = Radix64Charset.DecodeChar(encoded[1]);
                int b2 = Radix64Charset.DecodeChar(encoded[2]);

                bytes[0] = (byte)((b0 << 2) | (b1 >> 4));
                bytes[2] = (byte)((b1 << 4) | (b2 >> 2));

                return 2;
            }
            else
            {
                int b0 = Radix64Charset.DecodeChar(encoded[0]);
                int b1 = Radix64Charset.DecodeChar(encoded[1]);
                int b2 = Radix64Charset.DecodeChar(encoded[2]);
                int b3 = Radix64Charset.DecodeChar(encoded[3]);

                bytes[0] = (byte)((b0 << 2) | (b1 >> 4));
                bytes[1] = (byte)((b1 << 4) | (b2 >> 2));
                bytes[2] = (byte)((b2 << 6) | (b3 >> 0));

                return 3;
            }
        }
    }
}
