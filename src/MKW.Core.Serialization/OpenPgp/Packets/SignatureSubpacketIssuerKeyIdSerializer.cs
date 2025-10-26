// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    // https://www.rfc-editor.org/rfc/rfc4880#section-5.2.3.5
    //
    // 5.2.3.5.  Issuer
    // (8-octet Key ID)
    // The OpenPGP Key ID of the key issuing the signature.
    public static class SignatureSubpacketIssuerKeyIdSerializer
    {
        public const SignatureSubpacketTag Tag = SignatureSubpacketTag.IssuerKeyId;

        public static SignatureSubpacketIssuerKeyId Deserialize(SignatureSubpacketV4 subpacket)
        {
            IBufferReader<byte> reader = subpacket.CreateReader();

            return new SignatureSubpacketIssuerKeyId
            {
                KeyId = reader.ReadBytes(8),
            };
        }
    }
}
