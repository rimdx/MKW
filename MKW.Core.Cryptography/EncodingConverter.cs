using System.Text;

namespace MKW.Core.Cryptography
{
    public static class EncodingConverter
    {
        public static Memory<byte> GetBytes(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        public static string GetString(ReadOnlySpan<byte> data)
        {
            return Encoding.UTF8.GetString(data);
        }
    }
}
