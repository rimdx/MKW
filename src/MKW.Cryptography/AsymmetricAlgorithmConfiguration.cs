// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography
{
    public sealed record class AsymmetricAlgorithmConfiguration
    {
        public required AsymmetricAlgorithmEngine Engine { get; init; }
        public required HashAlgorithm HashEngine { get; init; }

        public required int StrengthBits { get; init; }
    }
}
