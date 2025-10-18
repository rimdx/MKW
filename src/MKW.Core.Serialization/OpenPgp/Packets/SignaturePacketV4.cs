// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public sealed record class SignaturePacketV4
    {
        public required SignatureTypeTag Type { get; init; }

        public required PublicKeyAlgorithmTag PublicKeyAlgorithm { get; init; }
        public required HashAlgorithmTag HashAlgorithm { get; init; }

        // See Computing Signatures (5.2.4):
        // - https://www.rfc-editor.org/rfc/rfc4880#section-5.2.4
        public required ReadOnlyMemory<byte> RawData { get; init; }
        public required ReadOnlyMemory<byte> Signature { get; init; }
    }
}
