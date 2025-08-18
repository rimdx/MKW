using System.Text;

namespace MKW.Core.Cryptography
{
    public static class EncodingConverter
    {
        public static byte[] GetBytes(string data)
        {
            return Encoding.Unicode.GetBytes(data);
        }

        public static string GetString(byte[] data)
        {
            return Encoding.Unicode.GetString(data);
        }
    }
}
