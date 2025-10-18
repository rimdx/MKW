// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public sealed record class SignaturePacketV4Body
    {
        public required IReadOnlyCollection<SignatureSubpacket> Subpackets { get; init; }
    }
}
