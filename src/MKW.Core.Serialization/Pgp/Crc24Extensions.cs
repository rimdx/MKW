using Org.BouncyCastle.Bcpg;

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
    }
}
