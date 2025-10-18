// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public sealed record class SignatureSubpacket
    {
        public required SignatureSubpacketTag Type { get; init; }
        public required ReadOnlyMemory<byte> RawData { get; init; }

        public ArrayBufferReader CreateReader()
        {
            return new ArrayBufferReader(RawData);
        }
    }
}
