// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;

namespace MKW.Storage
{
    public record class DatabaseUser
    {
        public required UserId Id { get; init; }

        public required ReadOnlyMemory<byte> Salt { get; init; }

        public required SignedPayload PublicKey { get; init; }
        public required SecretPayload PrivateKey { get; init; }

        public required SignedPayload Metadata { get; init; }
    }
}
