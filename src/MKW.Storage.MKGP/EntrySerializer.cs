using MKW.Core.Serialization.Pgp;
using MKW.Core.Serialization.Pgp.Packets;
using System.Buffers;

namespace MKW.Storage.MKGP
{
    public static class EntrySerializer
    {
        public static DatabaseEntry Deserialize(ArrayBufferReader reader)
        {
            throw new NotImplementedException();
        }

        public static void Serialize(IBufferWriter<byte> writer,
                                     DatabaseEntry obj)
        {
            //AsymmetricKeyParameter pubkey = PublicKeyFactory.CreateKey(entry.Data);

            SymEncryptedProtectedDataV1 packet = new SymEncryptedProtectedDataV1
            {
                Data = obj.Data,
            };

            SymEncryptedProtectedDataV1Serializer.SerializePacket(writer, packet, true);
        }
    }
}
