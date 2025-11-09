// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public sealed record class PublicKeyEncryptedSessionKeyV3 : PgpPacketBody
    {
        public required ReadOnlyMemory<byte> KeyId { get; init; }
        public required PublicKeyAlgorithmTag Tag { get; init; }
        public required ReadOnlyMemory<byte> Data { get; init; }

        public override T Visit<T>(IVisitor<T> visitor)
        {
            return visitor.VisitPublicKeyEncryptedSessionKeyV3(this);
        }
    }
}
