// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using Org.BouncyCastle.Bcpg;
using System.Buffers;
using System.Buffers.Binary;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    // https://www.rfc-editor.org/rfc/rfc4880#section-5.2.3.1
    // 
    // Each subpacket consists of a subpacket header and a body.  The header
    // consists of:
    // - the subpacket length (1, 2, or 5 octets),
    // - the subpacket type (1 octet),
    // 
    // and is followed by the subpacket-specific data.
    // 
    // The length includes the type octet but not this length.  Its format
    // is similar to the "new" format packet header lengths, but cannot have
    // Partial Body Lengths.  That is:
    // 
    // [...strip...]
    public static class SignatureSubpacketSerializer
    {
        public static void Serialize(IBufferWriter<byte> writer,
                                     SignatureSubpacket obj)
        {
            WriteBodyLength(writer, (uint)obj.RawData.Span.Length);
            writer.Write((byte)obj.Type);
            writer.Write(obj.RawData.Span);
        }

        private static void WriteBodyLength(IBufferWriter<byte> writer,
                                            uint bodyLen)
        {
            if (bodyLen < 192)
            {
                writer.Write((byte)bodyLen);
            }
            else if (bodyLen < ((0xFE - 192) << 8) + 0xFF + 192)
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

        private static uint ReadBodyLength(IBufferReader reader)
        {
            byte b0 = reader.ReadByte();

            if (b0 < 192)
            {
                return b0;
            }
            else if (b0 < 255)
            {
                Span<byte> data =
                [
                    (byte)(b0 - 192),
                    reader.ReadByte(),
                ];

                return BinaryPrimitives.ReadUInt16BigEndian(data) + 192U;
            }
            else
            {
                Span<byte> data =
                [
                    b0,
                    reader.ReadByte(),
                    reader.ReadByte(),
                    reader.ReadByte(),
                ];

                return BinaryPrimitives.ReadUInt32BigEndian(data);
            }
        }

        public static SignatureSubpacket Deserialize(IBufferReader reader)
        {
            int len = (int)ReadBodyLength(reader);
            SignatureSubpacketTag type = (SignatureSubpacketTag)reader.ReadByte();
            ReadOnlyMemory<byte> data = reader.ReadBytes(len);

            return new SignatureSubpacket
            {
                Type = type,
                RawData = data,
            };
        }
    }
}
