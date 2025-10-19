// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Cryptography;

namespace MKW.Storage
{
    public record class DatabaseConfiguration
    {
        public required SymmetricAlgorithmConfiguration PreferredSymmetricAlgorithm { get; init; }
        public required AsymmetricAlgorithmConfiguration PreferredPublicKeyAlgorithm { get; init; }
        public required PasswordDerivationConfiguration PreferredStringToKeyAlgorithm { get; init; }
    }
}
