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

            foreach (PgpPacketBody packet in PgpPacketReader.ReadAll(reader))
            {
                if (packet is PublicKeyEncryptedSessionKeyV3 sessionKey)
                {
                    users.Add(UserId.FromBytes(sessionKey.KeyId), sessionKey.Data);
                }
                else if (packet is SymEncryptedProtectedDataV1 protectedData)
                {
                    encryptedData = protectedData.Data;
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
                    KeyId = user.Key.GetBytes().EnsureSize(8),
                    Tag = PublicKeyAlgorithmTag.RsaGeneral,
                    Data = user.Value,
                };

                PgpPacketSerializer.Serialize(writer, PgpPacketBodySerializer.Serialize(sessionKey), false);
            }

            SymEncryptedProtectedDataV1 packet = new SymEncryptedProtectedDataV1
            {
                Data = obj.Data,
            };

            PgpPacketSerializer.Serialize(writer, PgpPacketBodySerializer.Serialize(packet), false);
        }
    }
}
