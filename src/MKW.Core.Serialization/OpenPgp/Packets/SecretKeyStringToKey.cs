// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public sealed record class SecretKeyStringToKey
    {
        public required SymmetricKeyAlgorithmTag SymmetricAlgorithm { get; init; }
        public required StringToKey StringToKey { get; init; }
    }
}
