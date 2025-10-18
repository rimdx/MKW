// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.Pgp
{
    public sealed record class PgpPacket(
        PacketTag Tag,
        ReadOnlyMemory<byte> EncodedBody
    );
}
