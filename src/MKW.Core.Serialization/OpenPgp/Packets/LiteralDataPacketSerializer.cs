// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Core.Serialization.OpenPgp.Primitives;
using System.Buffers;
using System.Text;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    internal static class LiteralDataPacketSerializer
    {
        private static class DataType
        {
            public const byte Binary = 0x62;
            public const byte Text = 0x74;
            public const byte Utf8Text = 0x75;
        }

        public static PgpPacketBody Deserialize(IBufferReader<byte> reader)
        {
            byte type = reader.ReadByte();
            byte fileNameLength = reader.ReadByte();
            string fileName = Encoding.UTF8.GetString(reader.ReadBytes(fileNameLength).ToArray());
            DateTime date = PgpDateTimeSerializer.Deserialize(reader);
            ReadOnlyMemory<byte> data = reader.ReadAll();

            return new LiteralDataPacket
            {
                Date = date,
                Data = data,
                DataType = type switch
                {
                    DataType.Binary => LiteralDataPacket.Type.Binary,
                    DataType.Text => LiteralDataPacket.Type.Text,
                    DataType.Utf8Text => LiteralDataPacket.Type.Utf8Text,
                    _ => throw new Exception("Unsupported literal data type.")
                },
                FileName = fileName,
            };
        }

        public static void Serialize(ArrayBufferWriter<byte> writer, LiteralDataPacket obj)
        {
            byte type = obj.DataType switch
            {
                LiteralDataPacket.Type.Binary => DataType.Binary,
                LiteralDataPacket.Type.Text => DataType.Text,
                LiteralDataPacket.Type.Utf8Text => DataType.Utf8Text,
            };

            ReadOnlyMemory<byte> fileName = Encoding.UTF8.GetBytes(obj.FileName);
            int fileNameLength = fileName.Length;
            if (fileNameLength > byte.MaxValue)
            {
                throw new Exception("Filename is too long.");
            }

            writer.Write(type);
            writer.Write((byte)fileNameLength);
            writer.Write(fileName.Span);
            PgpDateTimeSerializer.Serialize(writer, obj.Date);
            writer.Write(obj.Data.Span);
        }
    }
}