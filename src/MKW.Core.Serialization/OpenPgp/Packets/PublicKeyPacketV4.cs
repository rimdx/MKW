// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public sealed record class PublicKeyPacketV4
    {
        public required DateTime CreatedAt { get; init; }
        public required PublicKeyMaterial PublicKeyMaterial { get; init; }
    }
}
