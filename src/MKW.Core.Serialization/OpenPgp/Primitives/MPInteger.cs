// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using Org.BouncyCastle.Math;
using System.Buffers;
using System.Buffers.Binary;

namespace MKW.Core.Serialization.OpenPgp.Primitives
{
    public static class MPIntegerSerailizer
    {
        public static BigInteger Deserialize(IBufferReader reader)
        {
            ushort lengthInBits = BinaryPrimitives.ReadUInt16BigEndian(reader.ReadBytes(2).Span);
            int lengthInBytes = (lengthInBits + 7) / 8;

            ReadOnlyMemory<byte> bytes = reader.ReadBytes(lengthInBytes);

            return new BigInteger(1, bytes.ToArray());
        }

        public static void Serialize(IBufferWriter<byte> writer,
                                     BigInteger value)
        {
            if (value.SignValue < 0)
            {
                throw new ArgumentException("Values must be positive", nameof(value));
            }

            Span<byte> buf = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(buf, (ushort)value.BitLength);
            writer.Write(buf);

            writer.Write(value.ToByteArrayUnsigned());
        }
    }
}
