// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using Org.BouncyCastle.Bcpg;
using System.Buffers;
using System.Buffers.Binary;

namespace MKW.Core.Serialization.OpenPgp
{
    public static class PgpPacketSerializer
    {
        public static void Serialize(IBufferWriter<byte> writer,
                                     PgpPacket packet,
                                     bool oldFormat)
        {
            WriteHeader(writer,
                        packet.Tag,
                        packet.EncodedBody.Length,
                        oldFormat);

            writer.Write(packet.EncodedBody.Span);
        }

        private static void WriteHeader(IBufferWriter<byte> writer,
                                        PacketTag packetTag,
                                        long bodyLen,
                                        bool oldFormat)
        {
            if (oldFormat)
            {
                if (bodyLen <= 0xff)
                {
                    writer.Write(MakeOldHeader(packetTag, 0x00));
                }
                else if (bodyLen <= 0xffff)
                {
                    writer.Write(MakeOldHeader(packetTag, 0x01));
                }
                else
                {
                    writer.Write(MakeOldHeader(packetTag, 0x02));
                }

                WriteOldPacketLength(writer, bodyLen);
            }
            else
            {
                writer.Write(MakeNewHeader(packetTag));
                WriteNewPacketLength(writer, bodyLen);
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
            int newFormat = 1;

            int header =
                0b1000_0000 |
                0b0100_0000 * newFormat |
                0b0011_1111 & tag;

            return (byte)header;
        }

        private static void WriteNewPacketLength(IBufferWriter<byte> writer,
                                                 long bodyLen)
        {
            if (bodyLen < 192)
            {
                writer.Write((byte)bodyLen);
            }
            else if (bodyLen <= ((223 - 192) << 8) + 0xFF + 192)
            {
                bodyLen -= 192;

                writer.Write((byte)((bodyLen >> 8 & 0xff) + 192));
                writer.Write((byte)bodyLen);
            }
            else
            {
                Span<byte> buf = writer.GetSpan(5);
                buf[0] = 0xFF;
                BinaryPrimitives.WriteUInt32BigEndian(buf.Slice(1), (uint)bodyLen);
                writer.Advance(buf.Length);
            }
        }

        private static void WriteOldPacketLength(IBufferWriter<byte> writer,
                                                 long bodyLen)
        {
            if (bodyLen <= 0xff)
            {
                writer.Write((byte)bodyLen);
            }
            else if (bodyLen <= 0xffff)
            {
                Span<byte> buf = writer.GetSpan(2);
                BinaryPrimitives.WriteUInt16BigEndian(buf, (ushort)bodyLen);
                writer.Advance(buf.Length);
            }
            else
            {
                Span<byte> buf = writer.GetSpan(4);
                BinaryPrimitives.WriteUInt32BigEndian(buf, (uint)bodyLen);
                writer.Advance(buf.Length);
            }
        }

        private static uint ReadBodyLength(ArrayBufferReader reader,
                                           out bool partial)
        {
            byte b0 = reader.ReadByte();

            if (b0 < 192)
            {
                partial = false;
                return b0;
            }
            else if (b0 < 224)
            {
                Span<byte> data =
                [
                    (byte)(b0 - 192),
                    reader.ReadByte(),
                ];

                partial = false;
                return BinaryPrimitives.ReadUInt16BigEndian(data) + 192U;
            }
            else if (b0 == 255)
            {
                Span<byte> data =
                [
                    b0,
                    reader.ReadByte(),
                    reader.ReadByte(),
                    reader.ReadByte(),
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

        public static PgpPacket ReadPacket(ArrayBufferReader reader)
        {
            PacketTag tag;
            int bodyLen;

            byte header = reader.ReadByte();

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
                bodyLen = (int)ReadBodyLength(reader, out partial);
            }
            else
            {
                int lengthType = header & 0x3;
                tag = (PacketTag)((header & 0x3f) >> 2);

                switch (lengthType)
                {
                    case 0:
                        bodyLen = reader.ReadByte();
                        break;
                    case 1:
                        bodyLen = BinaryPrimitives.ReadUInt16BigEndian(reader.ReadBytes(2).Span);
                        break;
                    case 2:
                        bodyLen = (int)BinaryPrimitives.ReadUInt32BigEndian(reader.ReadBytes(4).Span);
                        break;
                    case 3:
                        bodyLen = 0;
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

            ReadOnlyMemory<byte> body = reader.ReadBytes(bodyLen);

            return new PgpPacket(tag, body);
        }
    }
}
