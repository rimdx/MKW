using MKW.Common;
using Org.BouncyCastle.Bcpg;
using System.Buffers.Binary;

namespace MKW.Core.Serialization.Pgp
{
    public sealed class PgpOutputStream : Stream, IDisposable
    {
        private readonly Stream proxy;
        private readonly bool oldFormat;

        public PgpOutputStream(Stream proxy)
        {
            this.proxy = proxy;
        }

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public override long Length => throw new NotImplementedException();
        public override long Position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override void Flush()
        {
            proxy.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            proxy.Write(buffer, offset, count);
        }

        public void WritePacket(PacketTag packetTag, PgpObject pgpObject)
        {
            using MemoryStream stream = new MemoryStream();
            using PgpOutputStream writer = new PgpOutputStream(stream);

            pgpObject.Encode(writer);

            WriteHeader(packetTag, writer.Length);

            stream.CopyTo(this);
        }

        private void WriteHeader(PacketTag packetTag, long bodyLen)
        {
            if (oldFormat)
            {
                if (bodyLen <= 0xff)
                {
                    proxy.WriteByte(MakeOldHeader(packetTag, 0x00));
                }
                else if (bodyLen <= 0xffff)
                {
                    proxy.WriteByte(MakeOldHeader(packetTag, 0x01));
                }
                else
                {
                    proxy.WriteByte(MakeOldHeader(packetTag, 0x02));
                }

                WriteOldPacketLength(bodyLen);
            }
            else
            {
                proxy.WriteByte(MakeNewHeader(packetTag));
                WriteNewPacketLength(bodyLen);
            }
        }

        private static byte MakeOldHeader(PacketTag packetTag, int lengthType)
        {
            //          PTag 7 6 5 4 3 2 1 0
            //
            // - Bit 7 -- Always one
            // - Bit 6 -- New packet format if set
            // Old format packets contain:
            // - Bits 5 - 2-- packet tag
            // - Bits 1 - 0-- length - type

            int tag = (int)packetTag;
            int newFormat = 0;

            int header =
                (0b1000_0000) |
                (0b0100_0000 * newFormat) |
                (0b0011_1100 & (tag << 2)) |
                (0b0000_0011 & (lengthType << 0));

            return (byte)header;
        }

        private static byte MakeNewHeader(PacketTag packetTag)
        {
            //          PTag 7 6 5 4 3 2 1 0
            //
            // - Bit 7 -- Always one
            // - Bit 6 -- New packet format if set
            // New format packets contain:
            // - Bits 5-0 -- packet tag

            int tag = (int)packetTag;
            int newFormat = 0;

            int header =
                (0b1000_0000) |
                (0b0100_0000 * newFormat) |
                (0b0011_1111 & tag);

            return (byte)header;
        }

        private void WriteNewPacketLength(long bodyLen)
        {
            if (bodyLen < 192)
            {
                proxy.WriteByte((byte)bodyLen);
            }
            else if (bodyLen <= 8383)
            {
                bodyLen -= 192;

                proxy.WriteByte((byte)(((bodyLen >> 8) & 0xff) + 192));
                proxy.WriteByte((byte)bodyLen);
            }
            else
            {
                Span<byte> buf = stackalloc byte[5];
                buf[0] = 0xFF;
                BinaryPrimitives.WriteUInt32BigEndian(buf.Slice(1), (uint)bodyLen);
                proxy.Write(buf);
            }
        }

        private void WriteOldPacketLength(long bodyLen)
        {
            if (bodyLen <= 0xff)
            {
                proxy.WriteByte((byte)bodyLen);
            }
            else if (bodyLen <= 0xffff)
            {
                Span<byte> buf = stackalloc byte[2];
                BinaryPrimitives.WriteUInt16BigEndian(buf, (ushort)bodyLen);
                proxy.Write(buf);
            }
            else
            {
                Span<byte> buf = stackalloc byte[4];
                BinaryPrimitives.WriteUInt32BigEndian(buf, (uint)bodyLen);
                proxy.Write(buf);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            proxy.Dispose();
        }
    }
}
