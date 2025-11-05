// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography
{
    public sealed record class PasswordDerivationConfiguration
    {
        public required PasswordDerivationEngine Engine { get; init; }

        public required HashAlgorithm HashEngine { get; init; }

        public required int SaltSizeBits { get; init; }
        public required int KeySizeBits { get; init; }

        public required int Iterations { get; init; }
    }
}
