// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Math;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public sealed record class SecretKeyMaterialRSA
    {
        public required BigInteger PrivateExponent { get; init; }
        public required BigInteger PrimeP { get; init; }
        public required BigInteger PrimeQ { get; init; }
        public required BigInteger Inverse { get; init; }
    }
}
