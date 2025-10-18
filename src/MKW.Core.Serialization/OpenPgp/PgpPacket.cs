// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.OpenPgp
{
    public sealed record class PgpPacket(
        PacketTag Tag,
        ReadOnlyMemory<byte> EncodedBody)
    {
        public ArrayBufferReader CreateReader()
        {
            return new ArrayBufferReader(EncodedBody);
        }
    }
}
