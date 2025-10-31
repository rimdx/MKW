// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Core.Serialization.OpenPgp.Packets;
using Org.BouncyCastle.Bcpg;
using System.Buffers;

namespace MKW.Storage.MKPG
{
    internal static class DatabaseTrustSignatureSerializer
    {
        public static DatabaseTrustSignature Deserialize(SignaturePacketV4 signaturePacket)
        {
            UserId? userId = null;
            ReadOnlyMemory<byte> signature;

            SignaturePacketV4Body body = SignaturePacketV4BodySerializer.Deserialize(signaturePacket.CreateReader());

            signature = signaturePacket.Signature;

            foreach (SignatureSubpacketV4 subpacket in body.Subpackets)
            {
                if (subpacket.Type == SignatureSubpacketIssuerKeyIdSerializer.Tag)
                {
                    SignatureSubpacketIssuerKeyId keyId = SignatureSubpacketIssuerKeyIdSerializer.Deserialize(subpacket);
                    userId = UserId.FromBytes(keyId.KeyId);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            if (userId == null)
            {
                throw new Exception("User ID is missing.");
            }

            return new DatabaseTrustSignature
            {
                Id = userId,
                SignatureBytes = signature,
            };
        }

        public static SignaturePacketV4 Serialize(DatabaseTrustSignature obj)
        {
            SignaturePacketV4Body body = new SignaturePacketV4Body
            {
                Subpackets =
                [
                    SignatureSubpacketIssuerKeyIdSerializer.Serialize(new SignatureSubpacketIssuerKeyId
                    {
                        KeyId = obj.Id.GetBytes(),
                    }),
                ],
            };

            ArrayBufferWriter<byte> signatureBodyWriter = new ArrayBufferWriter<byte>();
            SignaturePacketV4BodySerializer.Serialize(signatureBodyWriter, body);

            return new SignaturePacketV4
            {
                Type = SignatureTypeTag.PositiveCertificationPublicKeyPacket,
                PublicKeyAlgorithm = PublicKeyAlgorithmTag.RsaGeneral,
                HashAlgorithm = HashAlgorithmTag.Sha256,
                RawData = signatureBodyWriter.WrittenMemory,
                Signature = obj.SignatureBytes,
            };
        }
    }
}
