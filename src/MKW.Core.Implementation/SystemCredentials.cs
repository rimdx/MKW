// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Cryptography;

namespace MKW.Core.Implementation
{
    public record class SystemCredentials
    {
        public required ReadOnlyMemory<byte> Salt { get; set; }

        public required ReadOnlyMemory<byte> PublicKey { get; set; }

        public required SecretPayload EncryptedPrivateKey { get; set; }

        public required AsymmetricPrivateKey PrivateKey { get; set; }
    }
}
