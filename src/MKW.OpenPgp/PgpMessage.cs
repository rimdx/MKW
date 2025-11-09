// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Core.Serialization.OpenPgp;
using MKW.Core.Serialization.OpenPgp.Packets;
using MKW.Core.Serialization.OpenPgp.Primitives;
using MKW.Cryptography;
using Org.BouncyCastle.Bcpg;

namespace MKW.OpenPgp
{
    public sealed record class PgpMessage
    {
        private readonly ReadOnlyMemory<byte> encryptedPayload;
        private readonly ReadOnlyMemory<byte> encryptedSessionKey;

        private PgpMessage(ReadOnlyMemory<byte> encryptedPayload,
                           ReadOnlyMemory<byte> encryptedSessionKey)
        {
            this.encryptedPayload = encryptedPayload;
            this.encryptedSessionKey = encryptedSessionKey;
        }

        public static PgpMessage Open(PgpArmouredMessage msg)
        {
            ReadOnlyMemory<byte>? encryptedPayload = null;
            ReadOnlyMemory<byte>? encryptedSessionKey = null;

            IBufferReader<byte> reader = msg.CreateReader();

            while (reader.RemainingBytes > 0)
            {
                PgpPacket packet = PgpPacketSerializer.ReadPacket(reader);
                IBufferReader<byte> subreader = packet.CreateReader();

                if (packet.Tag == PacketTag.PublicKeyEncryptedSession)
                {
                    PublicKeyEncryptedSessionKeyV3 sessionKey = PublicKeyEncryptedSessionKeyV3Serializer.Deserialize(subreader);
                    encryptedSessionKey = sessionKey.Data;
                }
                else if (packet.Tag == PacketTag.SymmetricEncryptedIntegrityProtected)
                {
                    SymEncryptedProtectedDataV1 data = SymEncryptedProtectedDataV1Serializer.Deserialize(subreader);
                    encryptedPayload = data.Data;
                }
            }

            if (!encryptedPayload.HasValue || !encryptedSessionKey.HasValue)
            {
                throw new NullReferenceException();
            }

            return new PgpMessage(encryptedPayload.Value, encryptedSessionKey.Value);
        }

        public ReadOnlyMemory<byte> Decrypt(ICryptographyProvider crypto, IAsymmetricPrivateTransformer key)
        {
            ReadOnlyMemory<byte> mpi = MPIntegerSerailizer.DeserializeBytes(new ArrayBufferReader<byte>(encryptedSessionKey));

            ReadOnlyMemory<byte> sessionKeyBytes = key.Decrypt(mpi.Span);
            SessionKeyPayload sessionKeyPayload = SessionKeyPayloadSerializer.Deserialize(
                new ArrayBufferReader<byte>(sessionKeyBytes));

            SessionKeyProcessor processor = new SessionKeyProcessor(crypto);
            SymmetricKey sessionKey = processor.OpenKey(sessionKeyPayload);

            using ISymmetricTransformer sessionKeyTransformer = crypto.OpenSymmetricTransformer(sessionKey);

            ReadOnlyMemory<byte> payloadBytes = sessionKeyTransformer.Decrypt(encryptedPayload.Span);

            return payloadBytes;
        }
    }
}
