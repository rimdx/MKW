// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.Pgp.Packets
{
    public sealed record class StringToKeySaltedIterated : StringToKey
    {
        public required HashAlgorithmTag HashAlgorithmTag { get; init; }
        public required ReadOnlyMemory<byte> Salt { get; init; }
        public required byte Count { get; init; }
    }
}
