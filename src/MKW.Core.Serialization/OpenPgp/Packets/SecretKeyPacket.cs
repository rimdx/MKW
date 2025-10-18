// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public sealed record class SecretKeyPacket
    {
        public required PublicKeyPacketV4 PublicKey { get; init; }
        public required StringToKey StringToKey { get; init; }
    }
}
