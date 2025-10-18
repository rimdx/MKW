// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public sealed record class SignatureSubpacket
    {
        public required SignatureSubpacketTag Type { get; init; }
        public required ReadOnlyMemory<byte> RawData { get; init; }
    }
}
