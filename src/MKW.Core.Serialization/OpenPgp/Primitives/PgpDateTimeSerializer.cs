// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using System.Buffers;
using System.Buffers.Binary;

namespace MKW.Core.Serialization.OpenPgp.Primitives
{
    public static class PgpDateTimeSerializer
    {
        private static readonly DateTime origin = new DateTime(
            1970 /* year */,
            1 /* month */,
            1 /* day */,
            0 /* hour */,
            0 /* minute */,
            0 /* second */,
            0 /* millisecond */,
            DateTimeKind.Utc
        );

        public static void Serialize(IBufferWriter<byte> writer,
                                     DateTime dateTime)
        {
            TimeSpan difference = dateTime - origin;
            uint unixTime = ((uint)difference.TotalSeconds) & 0xFFFF_FFFF;

            BinaryPrimitives.WriteUInt32BigEndian(writer.GetSpan(4), unixTime);
        }

        public static DateTime Deserialize(ArrayBufferReader reader)
        {
            int unixTime = BinaryPrimitives.ReadInt32BigEndian(reader.ReadBytes(4).Span);
            return origin.AddSeconds(unixTime);
        }
    }
}
