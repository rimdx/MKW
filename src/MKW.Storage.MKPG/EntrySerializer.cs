// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Core;
using MKW.Core.Serialization.OpenPgp;
using MKW.Core.Serialization.OpenPgp.Packets;
using Org.BouncyCastle.Bcpg;
using System.Buffers;

namespace MKW.Storage.MKPG
{
    public static class EntrySerializer
    {
        public static DatabaseEntry Deserialize(IBufferReader<byte> reader, EntryId entryId)
        {
            Dictionary<UserId, ReadOnlyMemory<byte>> users = [];
            ReadOnlyMemory<byte>? encryptedData = null;

            while (reader.RemainingBytes > 0)
            {
                PgpPacket packet = PgpPacketSerializer.ReadPacket(reader);
                IBufferReader<byte> subreader = packet.CreateReader();

                if (packet.Tag == PacketTag.PublicKeyEncryptedSession)
                {
                    PublicKeyEncryptedSessionKeyV3 sessionKey = PublicKeyEncryptedSessionKeyV3Serializer.Deserialize(subreader);

                    users.Add(UserId.FromBytes(sessionKey.KeyId), sessionKey.Data);
                }
                else if (packet.Tag == PacketTag.SymmetricEncryptedIntegrityProtected)
                {
                    SymEncryptedProtectedDataV1 data = SymEncryptedProtectedDataV1Serializer.Deserialize(subreader);

                    encryptedData = data.Data;
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            if (encryptedData == null)
            {
                throw new Exception("Missing data packet.");
            }

            return new DatabaseEntry
            {
                Id = entryId,
                Data = encryptedData.Value,
                Salt = null,
                Keys = users,
            };
        }

        public static void Serialize(IBufferWriter<byte> writer,
                                     DatabaseEntry obj)
        {
            //AsymmetricKeyParameter pubkey = PublicKeyFactory.CreateKey(entry.Data);

            foreach (KeyValuePair<UserId, ReadOnlyMemory<byte>> user in obj.Keys)
            {
                PublicKeyEncryptedSessionKeyV3 sessionKey = new PublicKeyEncryptedSessionKeyV3
                {
                    KeyId = user.Key.GetBytes().Slice(0, 8),
                    Tag = PublicKeyAlgorithmTag.RsaGeneral,
                    Data = user.Value,
                };

                PublicKeyEncryptedSessionKeyV3Serializer.SerializePacket(writer, sessionKey, false);
            }

            SymEncryptedProtectedDataV1 packet = new SymEncryptedProtectedDataV1
            {
                Data = obj.Data,
            };

            SymEncryptedProtectedDataV1Serializer.SerializePacket(writer, packet, false);
        }
    }
}
