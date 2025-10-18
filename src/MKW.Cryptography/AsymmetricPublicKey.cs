// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography
{
    public sealed record class AsymmetricPublicKey
    {
        public required ReadOnlyMemory<byte> Modulus { get; init; }
        public required ReadOnlyMemory<byte> PublicExponent { get; init; }
    }
}
