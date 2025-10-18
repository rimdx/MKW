// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.Pgp.Packets
{
    public sealed record class SymEncryptedProtectedDataV1
    {
        public required ReadOnlyMemory<byte> Data { get; init; }
    }
}
