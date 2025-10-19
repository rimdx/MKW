// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;

namespace MKW.Storage
{
    // Represents detached instance of trust signature, in which admin confirms
    // user's identity.
    //
    // Signature is performed using admin's private key over one's public key.
    //
    // Future versions may also concatenate User ID (metadata) as well.
    public sealed record class DatabaseTrustSignature
    {
        public required UserId Id { get; init; }
        public required ReadOnlyMemory<byte> SignatureBytes { get; init; }
    }
}
