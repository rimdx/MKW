// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public sealed record class SecretKeyPacketV4 : PgpPacketBody
    {
        public required PublicKeyPacketV4 PublicKey { get; init; }
        public required SecretKeyStringToKey StringToKey { get; init; }
        public required ReadOnlyMemory<byte> SecretKeyData { get; init; }

        public override T Visit<T>(IVisitor<T> visitor) => visitor.VisitSecretKeyPacketV4(this);
    }
}
