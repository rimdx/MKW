// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography
{
    public sealed record class SymmetricKey
    {
        public required SymmetricAlgorithmEngine Engine { get; init; }

        public required ReadOnlyMemory<byte> KeyBytes { get; init; }
        public required ReadOnlyMemory<byte> IVBytes { get; init; }
    }
}
