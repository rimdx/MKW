// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Math;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public sealed record class PublicKeyMaterialRSA : PublicKeyMaterial
    {
        public required BigInteger Modulus { get; init; }
        public required BigInteger PublicExponent { get; init; }
    }
}
