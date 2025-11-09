// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.OpenPgp.MDP
{
    public sealed record class OpenPgpModificationDetectionPacket
    {
        public required ReadOnlyMemory<byte> Salt { get; init; }
        public required ReadOnlyMemory<byte> Plaintext { get; init; }
        public required ReadOnlyMemory<byte> Checksum { get; init; }
    }
}
