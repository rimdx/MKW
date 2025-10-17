namespace MKW.Core.Serialization.Pgp
{
    internal static class Radix64Charset
    {
        public const byte Padding = (byte)'=';

        public static readonly byte[] EncodingTable =
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

        public readonly static ReadOnlyMemory<byte> DecodingTable = CreateDecodingTable();

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

    }
}
