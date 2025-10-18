// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public sealed record class StringToKeySimple : StringToKey
    {
        public required HashAlgorithmTag HashAlgorithmTag { get; init; }
    }
}
