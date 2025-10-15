using MKW.Common;
using Org.BouncyCastle.Bcpg;
using System.Buffers.Binary;

namespace MKW.Core.Serialization.Pgp
{
    public sealed class PgpInputStream : Stream, IDisposable
    {
        private readonly Stream proxy;

        public PgpInputStream(Stream proxy)
        {
            this.proxy = proxy;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
            proxy.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return proxy.Read(buffer, offset, count);
        }

        public byte RequireByte()
        {
            int b = ReadByte();

            if (b < 0)
            {
                throw new EndOfStreamException();
            }
            else
            {
                return (byte)b;
            }
        }

        public ReadOnlyMemory<byte> ReadExact(int count)
        {
            byte[] data = new byte[count];
            int len = Read(data, 0, data.Length);

            if (len < count)
            {
                throw new EndOfStreamException();
            }
            else
            {
                return data;
            }
        }

        private uint ReadBodyLength(out bool partial)
        {
            int b0 = ReadByte();

            if (b0 < 0)
            {
                partial = false;
                return 0U;
            }
            else if (b0 < 192)
            {
                partial = false;
                return (uint)b0;
            }
            else if (b0 < 224)
            {
                Span<byte> data =
                [
                    (byte)b0,
                    (byte)ReadByte(),
                ];

                partial = false;
                return BinaryPrimitives.ReadUInt16BigEndian(data);
            }
            else if (b0 == 255)
            {
                Span<byte> data =
                [
                    (byte)b0,
                    (byte)ReadByte(),
                    (byte)ReadByte(),
                    (byte)ReadByte(),
                ];

                partial = false;
                return BinaryPrimitives.ReadUInt32BigEndian(data);
            }
            else
            {
                partial = true;
                return 1U << (b0 & 0x1F);
            }
        }

        public PgpInputStream ReadPacket(out PacketTag tag, out uint bodyLen)
        {
            int header = ReadByte();

            if (header < 0)
            {
                throw new EndOfStreamException();
            }

            if ((header & 0x80) == 0)
            {
                throw new IOException("invalid header encountered");
            }

            bool newPacket = (header & 0x40) != 0;
            bool partial = false;

            if (newPacket)
            {
                tag = (PacketTag)(header & 0x3f);
                bodyLen = ReadBodyLength(out partial);
            }
            else
            {
                int lengthType = header & 0x3;
                tag = (PacketTag)((header & 0x3f) >> 2);

                switch (lengthType)
                {
                    case 0:
                        bodyLen = RequireByte();
                        break;
                    case 1:
                        bodyLen = BinaryPrimitives.ReadUInt16BigEndian(ReadExact(2).Span);
                        break;
                    case 2:
                        bodyLen = BinaryPrimitives.ReadUInt32BigEndian(ReadExact(4).Span);
                        break;
                    case 3:
                        bodyLen = 0U;
                        partial = true;
                        break;
                    default:
                        throw new IOException("unknown length type encountered");
                }
            }

            if (partial)
            {
                throw new NotImplementedException();
            }

            PartialInputStream pis = new PartialInputStream(this, bodyLen);
            Stream buf = new BufferedStream(pis);
            return new PgpInputStream(buf);
        }

        public ReadOnlyMemory<byte> ReadAll()
        {
            MemoryStream buf = new MemoryStream();
            proxy.CopyTo(buf);
            return buf.ToArray();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            proxy.Dispose();
        }
    }
}
