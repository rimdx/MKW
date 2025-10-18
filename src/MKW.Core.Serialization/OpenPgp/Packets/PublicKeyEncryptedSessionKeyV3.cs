// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public sealed record class PublicKeyEncryptedSessionKeyV3
    {
        public required ReadOnlyMemory<byte> KeyId { get; init; }
        public required PublicKeyAlgorithmTag Tag { get; init; }
        public required ReadOnlyMemory<byte> Data { get; init; }
    }
}
