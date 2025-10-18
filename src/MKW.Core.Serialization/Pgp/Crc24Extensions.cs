using Org.BouncyCastle.Bcpg;
using System.Text;

namespace MKW.Core.Serialization.Pgp
{
    public static class Crc24Extensions
    {
        public static void Update(this Crc24 crc, ReadOnlySpan<byte> data)
        {
            byte[] buf = new byte[3];

            int i = 0;
            for (; i + 3 < data.Length; i += 3)
            {
                buf[0] = data[i + 0];
                buf[1] = data[i + 1];
                buf[2] = data[i + 2];
                crc.Update3(buf, 0);
            }

            for (; i < data.Length; i++)
            {
                crc.Update(data[i]);
            }
        }

        public static string Serialize(this Crc24 crc)
        {
            byte[] buf = new byte[3];
            byte[] encoded = new byte[4];

            int value = crc.Value;
            buf[0] = (byte)(0xFF & (value >> 16));
            buf[1] = (byte)(0xFF & (value >> 8));
            buf[2] = (byte)(0xFF & (value >> 0));

            Radix64BitConvert.EncodeFullBlock(buf, encoded);

            return Encoding.ASCII.GetString(encoded);
        }
    }
}
