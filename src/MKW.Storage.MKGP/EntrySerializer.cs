using MKW.Core;
using MKW.Core.Serialization.Pgp;
using MKW.Core.Serialization.Pgp.Packets;
using Org.BouncyCastle.Bcpg;
using System.Buffers;

namespace MKW.Storage.MKGP
{
    public static class EntrySerializer
    {
        public static DatabaseEntry Deserialize(ArrayBufferReader reader)
        {
            Dictionary<UserId, ReadOnlyMemory<byte>> users = [];
            ReadOnlyMemory<byte>? encryptedData = null;

            while (reader.RemainingBytes > 0)
            {
                PgpPacket packet = PgpPacketSerializer.ReadPacket(reader);
                ArrayBufferReader subreader = new ArrayBufferReader(packet.EncodedBody);

                if (packet.Tag == PacketTag.PublicKeyEncryptedSession)
                {
                    PublicKeyEncryptedSessionKeyV3 sessionKey = PublicKeyEncryptedSessionKeyV3Serializer.Deserialize(subreader);

                    Guid id = new Guid([
                        ..new byte[8],
                        ..sessionKey.KeyId.Span,
                    ]);

                    users.Add(UserId.FromGuid(id), sessionKey.Data);
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
                Id = EntryId.Create(),
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
                    KeyId = user.Key.GetGuid().ToByteArray().AsMemory().Slice(0, 8),
                    Tag = PublicKeyAlgorithmTag.RsaGeneral,
                    Data = user.Value,
                };

                PublicKeyEncryptedSessionKeyV3Serializer.SerializePacket(writer, sessionKey, true);
            }

            SymEncryptedProtectedDataV1 packet = new SymEncryptedProtectedDataV1
            {
                Data = obj.Data,
            };

            SymEncryptedProtectedDataV1Serializer.SerializePacket(writer, packet, true);
        }
    }
}
