// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Core;
using MKW.Core.Serialization.OpenPgp;
using MKW.Core.Serialization.OpenPgp.Packets;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Buffers;

namespace MKW.Storage.MKPG
{
    public static class UserSerializer
    {
        public static DatabaseUser Deserialize(IBufferReader<byte> reader, UserId userId)
        {
            ReadOnlyMemory<byte>? seckey = null;
            ReadOnlyMemory<byte>? pubkey = null;
            ReadOnlyMemory<byte>? pubkeySignature = null;
            ReadOnlyMemory<byte>? salt = null;
            ReadOnlyMemory<byte>? metadata = null;

            while (reader.RemainingBytes > 0)
            {
                PgpPacket packet = PgpPacketSerializer.ReadPacket(reader);
                IBufferReader<byte> subreader = packet.CreateReader();

                if (packet.Tag == PacketTag.SecretKey)
                {
                    SecretKeyPacketV4 seckeyPacket = SecretKeyPacketV4Serializer.Deserialize(subreader);
                    PublicKeyMaterialRSA pubkeyMaterial = (PublicKeyMaterialRSA)seckeyPacket.PublicKey.PublicKeyMaterial;
                    StringToKeySaltedIterated s2k = (StringToKeySaltedIterated)seckeyPacket.StringToKey.StringToKey;

                    // TODO: workaround!
                    AlgorithmIdentifier algID = new AlgorithmIdentifier(PkcsObjectIdentifiers.RsaEncryption, DerNull.Instance);
                    RsaPublicKeyStructure structure = new RsaPublicKeyStructure(pubkeyMaterial.Modulus, pubkeyMaterial.PublicExponent);
                    SubjectPublicKeyInfo parameters = new SubjectPublicKeyInfo(algID, structure);

                    seckey = seckeyPacket.SecretKeyData;
                    pubkey = parameters.GetEncoded();
                    salt = s2k.Salt;
                }
                else if (packet.Tag == PacketTag.Signature)
                {
                    SignaturePacketV4 signaturePacket = SignaturePacketV4Serializer.Deserialize(subreader);

                    if (signaturePacket.Type == SignatureTypeTag.PositiveCertificationPublicKeyPacket)
                    {
                        SignaturePacketV4Body body = SignaturePacketV4BodySerializer.Deserialize(signaturePacket.CreateReader());

                        pubkeySignature = signaturePacket.Signature;
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }
                else if (packet.Tag == UserIdPacketSerializer.Tag)
                {
                    Core.Serialization.OpenPgp.Packets.UserIdPacket userIdPacket = UserIdPacketSerializer.Deserialize(subreader);
                    metadata = userIdPacket.Content;
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            if (pubkey == null)
            {
                throw new Exception("Public key is missing.");
            }

            if (pubkeySignature == null)
            {
                throw new Exception("Public key signature is missing.");
            }

            if (seckey == null)
            {
                throw new Exception("Encrypted secret key is missing.");
            }

            if (salt == null)
            {
                throw new Exception("Public salt is missing.");
            }

            if (metadata == null)
            {
                throw new Exception("Metadata is missing.");
            }

            return new DatabaseUser
            {
                Id = userId,
                PublicKey = new SignedPayload(pubkey.Value, pubkeySignature.Value),
                PrivateKey = new SecretPayload(seckey.Value),
                Salt = salt.Value,
                Metadata = new SignedPayload(metadata.Value, pubkeySignature.Value),
            };
        }

        public static void Serialize(IBufferWriter<byte> writer,
                                     DatabaseUser obj)
        {
            // TODO: workaround!
            RsaKeyParameters decoded = (RsaKeyParameters)PublicKeyFactory.CreateKey(obj.PublicKey.Payload.ToArray());

            SecretKeyPacketV4 seckeyPacket = new SecretKeyPacketV4
            {
                PublicKey = new PublicKeyPacketV4
                {
                    CreatedAt = new DateTime(42),
                    PublicKeyMaterial = new PublicKeyMaterialRSA
                    {
                        Modulus = decoded.Modulus,
                        PublicExponent = decoded.Exponent,
                    },
                },
                StringToKey = new SecretKeyStringToKey
                {
                    SymmetricAlgorithm = SymmetricKeyAlgorithmTag.Aes128,
                    StringToKey = new StringToKeySaltedIterated
                    {
                        HashAlgorithmTag = HashAlgorithmTag.Sha256,
                        Count = 42,
                        Salt = obj.Salt,
                    },
                },
                SecretKeyData = obj.PrivateKey.EncryptedPayload,
            };

            ArrayBufferWriter<byte> seckeyPacketWriter = new ArrayBufferWriter<byte>();
            SecretKeyPacketV4Serializer.Serialize(seckeyPacketWriter, seckeyPacket);
            PgpPacketSerializer.Serialize(writer,
                                          new PgpPacket(PacketTag.SecretKey, seckeyPacketWriter.WrittenMemory),
                                          false);

            ArrayBufferWriter<byte> signatureBodyWriter = new ArrayBufferWriter<byte>();
            SignaturePacketV4Body body = new SignaturePacketV4Body
            {
                Subpackets = [],
            };
            SignaturePacketV4BodySerializer.Serialize(signatureBodyWriter, body);

            SignaturePacketV4 signaturePacket = new SignaturePacketV4
            {
                Type = SignatureTypeTag.PositiveCertificationPublicKeyPacket,
                PublicKeyAlgorithm = PublicKeyAlgorithmTag.RsaGeneral,
                HashAlgorithm = HashAlgorithmTag.Sha256,
                RawData = signatureBodyWriter.WrittenMemory,
                Signature = obj.PublicKey.Signature,
            };
            ArrayBufferWriter<byte> signaturePacketWriter = new ArrayBufferWriter<byte>();
            SignaturePacketV4Serializer.Serialize(signaturePacketWriter, signaturePacket);

            PgpPacketSerializer.Serialize(writer,
                                          new PgpPacket(PacketTag.Signature, signaturePacketWriter.WrittenMemory),
                                          false);

            ArrayBufferWriter<byte> userIdPacketWriter = new ArrayBufferWriter<byte>();
            Core.Serialization.OpenPgp.Packets.UserIdPacket userIdPacket = new Core.Serialization.OpenPgp.Packets.UserIdPacket(obj.Metadata.Payload);
            UserIdPacketSerializer.Serialize(userIdPacketWriter, userIdPacket);

            PgpPacketSerializer.Serialize(writer,
                                          new PgpPacket(UserIdPacketSerializer.Tag, userIdPacketWriter.WrittenMemory),
                                          false);
        }
    }
}
