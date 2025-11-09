// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public sealed record class SymEncryptedProtectedDataV1 : PgpPacketBody
    {
        public required ReadOnlyMemory<byte> Data { get; init; }

        public override T Visit<T>(IVisitor<T> visitor)
        {
            return visitor.VisitSymEncryptedProtectedDataV1(this);
        }
    }
}
