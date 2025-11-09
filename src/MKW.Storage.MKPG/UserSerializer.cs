// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Core;
using MKW.Core.Serialization.OpenPgp;
using MKW.Core.Serialization.OpenPgp.Packets;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
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
            List<DatabaseTrustSignature> signatures = [];
            ReadOnlyMemory<byte>? salt = null;
            ReadOnlyMemory<byte>? metadata = null;

            foreach (PgpPacketBody packet in PgpPacketReader.ReadAll(reader))
            {
                if (packet is SecretKeyPacketV4 seckeyPacket)
                {
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
                else if (packet is SignaturePacketV4 signaturePacket)
                {
                    if (signaturePacket.Type == SignatureTypeTag.PositiveCertificationPublicKeyPacket)
                    {
                        signatures.Add(DatabaseTrustSignatureSerializer.Deserialize(signaturePacket));
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }
                else if (packet is Core.Serialization.OpenPgp.Packets.UserIdPacket userIdPacket)
                {
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
                ProtectedData = new DatabaseUserProtectedDataSigned
                {
                    PublicKey = pubkey.Value,
                    Metadata = metadata.Value,
                    Signature = signatures,
                },
                PrivateKey = new SecretPayload(seckey.Value),
                Salt = salt.Value,
            };
        }

        public static void Serialize(IBufferWriter<byte> writer,
                                     DatabaseUser obj)
        {
            // TODO: workaround!
            RsaKeyParameters decoded = (RsaKeyParameters)PublicKeyFactory.CreateKey(obj.ProtectedData.PublicKey.ToArray());

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

            PgpPacketSerializer.Serialize(writer, PgpPacketBodySerializer.Serialize(seckeyPacket), false);

            foreach (DatabaseTrustSignature signature in obj.ProtectedData.Signature)
            {
                SignaturePacketV4 signaturePacket = DatabaseTrustSignatureSerializer.Serialize(signature);

                PgpPacketSerializer.Serialize(writer,
                                              PgpPacketBodySerializer.Serialize(signaturePacket),
                                              false);
            }

            Core.Serialization.OpenPgp.Packets.UserIdPacket userIdPacket = new Core.Serialization.OpenPgp.Packets.UserIdPacket(obj.ProtectedData.Metadata);

            PgpPacketSerializer.Serialize(writer,
                                          PgpPacketBodySerializer.Serialize(userIdPacket),
                                          false);
        }
    }
}
