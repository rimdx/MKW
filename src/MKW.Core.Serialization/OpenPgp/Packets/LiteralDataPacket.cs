// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Text;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public record class LiteralDataPacket : PgpPacketBody
    {
        public enum Type
        {
            Binary, Text, Utf8Text
        }

        public required DateTime Date { get; init; }
        public required ReadOnlyMemory<byte> Data { get; init; }
        public required Type DataType { get; init; }
        public required string FileName { get; init; }

        public override T Visit<T>(IVisitor<T> visitor)
        {
            return visitor.VisitLiteralData(this);
        }

        public string GetAsString()
        {
            return Encoding.UTF8.GetString(Data.ToArray());
        }
    }
}