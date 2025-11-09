// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public sealed record class PublicKeyPacketV4 : PgpPacketBody
    {
        public required DateTime CreatedAt { get; init; }
        public required PublicKeyMaterial PublicKeyMaterial { get; init; }

        public override T Visit<T>(IVisitor<T> visitor)
        {
            return visitor.VisitPublicKeyPacketV4(this);
        }
    }
}
