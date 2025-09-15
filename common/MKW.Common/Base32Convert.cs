using System.Security.Cryptography;
using System.Text;

namespace MKW.Common
{
    public static class Base32Convert
    {
        public static string Encode(ReadOnlySpan<byte> data)
        {
            using ICryptoTransform transform = new Base32EncodeTransformer();

            using MemoryStream output = new MemoryStream();
            using CryptoStream cryptoStream = new CryptoStream(output, transform, CryptoStreamMode.Write);

            cryptoStream.Write(data);
            cryptoStream.FlushFinalBlock();

            return Encoding.UTF8.GetString(output.ToArray());
        }
    }
}
