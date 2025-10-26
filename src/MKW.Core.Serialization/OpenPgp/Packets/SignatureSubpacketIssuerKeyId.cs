// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public sealed record class SignatureSubpacketIssuerKeyId
    {
        // 8 octets
        public required ReadOnlyMemory<byte> KeyId;
    }
}
