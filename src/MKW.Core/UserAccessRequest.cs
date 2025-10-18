// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core
{
    public record class UserAccessRequest
    {
        /* plain */
        public required ReadOnlyMemory<byte> Salt { get; init; }
        public required ReadOnlyMemory<byte> PublicKey { get; init; }

        /* encrypted */
        public required SecretPayload EncryptedPrivateKey { get; init; }

        public required ReadOnlyMemory<byte> AdminSignature { get; init; }
    }
}
