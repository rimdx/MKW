// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    // https://www.rfc-editor.org/rfc/rfc4880#section-5.2.1
    //
    // 0x00: Signature of a binary document.
    // 0x01: Signature of a canonical text document.
    // 0x02: Standalone signature.
    // 0x10: Generic certification of a User ID and Public-Key packet.
    // 0x11: Persona certification of a User ID and Public-Key packet.
    // 0x12: Casual certification of a User ID and Public-Key packet.
    // 0x13: Positive certification of a User ID and Public-Key packet.
    // 0x18: Subkey Binding Signature
    // 0x19: Primary Key Binding Signature
    // 0x1F: Signature directly on a key
    // 0x20: Key revocation signature
    // 0x28: Subkey revocation signature
    // 0x30: Certification revocation signature
    // 0x40: Timestamp signature.
    // 0x50: Third-Party Confirmation signature.
    public enum SignatureTypeTag : byte
    {
        SignatureBinaryDocument = 0x00,
        SignatureCanonicalTextDocument = 0x01,
        StandaloneSignature = 0x02,

        GenericCertificationPublicKeyPacket = 0x10,
        PersonaCertificationPublicKeyPacket = 0x11,
        CasualCertificationPublicKeyPacket = 0x12,
        PositiveCertificationPublicKeyPacket = 0x13,

        SubkeyBindingSignature = 0x18,
        PrimaryKeyBindingSignature = 0x19,

        SignatureDirectlyOnKey = 0x1F,

        KeyRevocationSignature = 0x20,
        SubkeyRevocationSignature = 0x28,
        CertificationRevocationSignature = 0x30,

        TimestampSignature = 0x40,

        ThirdPartyConfirmationSignature = 0x50,
    }
}
