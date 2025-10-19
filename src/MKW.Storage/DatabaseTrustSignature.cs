// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;

namespace MKW.Storage
{
    // Represents detached instance of trust signature, in which users confirm
    // admins's identity.
    //
    // Signature is performed using one's private key over admin's public key.
    public sealed record class DatabaseTrustSignature
    {
        public required UserId Id { get; init; }
        public required ReadOnlyMemory<byte> SignatureBytes { get; init; }
    }
}
