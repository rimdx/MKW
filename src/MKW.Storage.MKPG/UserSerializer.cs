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
        public static DatabaseUser Deserialize(IBufferReader reader)
        {
            ReadOnlyMemory<byte>? seckey = null;
            ReadOnlyMemory<byte>? pubkey = null;
            ReadOnlyMemory<byte>? salt = null;

            while (reader.RemainingBytes > 0)
            {
                PgpPacket packet = PgpPacketSerializer.ReadPacket(reader);
                IBufferReader subreader = packet.CreateReader();

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
                else
                {
                    throw new NotSupportedException();
                }
            }

            if (pubkey == null)
            {
                throw new Exception("Public key is missing.");
            }

            if (seckey == null)
            {
                throw new Exception("Encrypted secret key is missing.");
            }

            if (salt == null)
            {
                throw new Exception("Public salt is missing.");
            }

            return new DatabaseUser
            {
                Id = null,
                PublicKey = new SignedPayload(pubkey.Value, null),
                PrivateKey = new SecretPayload(seckey.Value),
                Salt = salt.Value,
                AdminSignature = null,
                Metadata = null,
            };
        }

        public static void Serialize(IBufferWriter<byte> writer,
                                     DatabaseUser obj)
        {
            // TODO: workaround!
            RsaKeyParameters decoded = (RsaKeyParameters)PublicKeyFactory.CreateKey(obj.PublicKey.Payload.ToArray());

            SecretKeyPacketV4 packet = new SecretKeyPacketV4
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

            ArrayBufferWriter<byte> packetWriter = new ArrayBufferWriter<byte>();
            SecretKeyPacketV4Serializer.Serialize(packetWriter, packet);
            PgpPacketSerializer.Serialize(writer,
                                          new PgpPacket(PacketTag.SecretKey, packetWriter.WrittenMemory),
                                          false);
        }
    }
}
