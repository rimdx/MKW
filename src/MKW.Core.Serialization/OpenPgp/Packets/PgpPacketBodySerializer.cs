// Copyrighpublic t (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using Org.BouncyCastle.Bcpg;
using System.Buffers;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public static class PgpPacketBodySerializer
    {
        public static PgpPacket Serialize(PgpPacketBody packetBody)
        {
            return packetBody.Visit(new SerializeVisitor());
        }

        public static PgpPacketBody Deserialize(PgpPacket packet)
        {
            IBufferReader<byte> subreader = packet.CreateReader();

            if (packet.Tag == PacketTag.SecretKey)
            {
                return SecretKeyPacketV4Serializer.Deserialize(subreader);
            }
            else if (packet.Tag == PacketTag.Signature)
            {
                return SignaturePacketV4Serializer.Deserialize(subreader);
            }
            else if (packet.Tag == UserIdPacketSerializer.Tag)
            {
                return UserIdPacketSerializer.Deserialize(subreader);
            }
            else if (packet.Tag == PacketTag.PublicKeyEncryptedSession)
            {
                return PublicKeyEncryptedSessionKeyV3Serializer.Deserialize(subreader);
            }
            else if (packet.Tag == PacketTag.SymmetricEncryptedIntegrityProtected)
            {
                return SymEncryptedProtectedDataV1Serializer.Deserialize(subreader);
            }
            else if (packet.Tag == PacketTag.PublicKey)
            {
                return PublicKeyPacketV4Serializer.Deserialize(subreader);
            }
            else if (packet.Tag == PacketTag.LiteralData)
            {
                return LiteralDataPacketSerializer.Deserialize(subreader);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private sealed class SerializeVisitor : PgpPacketBody.IVisitor<PgpPacket>
        {
            public PgpPacket VisitLiteralData(LiteralDataPacket packetBody)
            {
                ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();
                LiteralDataPacketSerializer.Serialize(writer, packetBody);
                return new PgpPacket(PacketTag.LiteralData, writer.WrittenMemory);
            }

            public PgpPacket VisitPublicKeyEncryptedSessionKeyV3(PublicKeyEncryptedSessionKeyV3 packetBody)
            {
                ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();
                PublicKeyEncryptedSessionKeyV3Serializer.Serialize(writer, packetBody);
                return new PgpPacket(PacketTag.PublicKeyEncryptedSession, writer.WrittenMemory);
            }

            public PgpPacket VisitPublicKeyPacketV4(PublicKeyPacketV4 packetBody)
            {
                ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();
                PublicKeyPacketV4Serializer.Serialize(writer, packetBody);
                return new PgpPacket(PacketTag.PublicKey, writer.WrittenMemory);
            }

            public PgpPacket VisitSecretKeyPacketV4(SecretKeyPacketV4 packetBody)
            {
                ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();

                SecretKeyPacketV4Serializer.Serialize(writer, packetBody);

                return new PgpPacket(PacketTag.SecretKey, writer.WrittenMemory);
            }

            public PgpPacket VisitSignaturePacketV4(SignaturePacketV4 packetBody)
            {
                ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();

                SignaturePacketV4Serializer.Serialize(writer, packetBody);

                return new PgpPacket(PacketTag.Signature, writer.WrittenMemory);
            }

            public PgpPacket VisitSymEncryptedProtectedDataV1(SymEncryptedProtectedDataV1 packetBody)
            {
                ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();

                SymEncryptedProtectedDataV1Serializer.Serialize(writer, packetBody);

                return new PgpPacket(PacketTag.SymmetricEncryptedIntegrityProtected, writer.WrittenMemory);
            }

            public PgpPacket VisitUserIdPacket(UserIdPacket packetBody)
            {
                ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();

                UserIdPacketSerializer.Serialize(writer, packetBody);

                return new PgpPacket(UserIdPacketSerializer.Tag, writer.WrittenMemory);
            }
        }
    }
}
