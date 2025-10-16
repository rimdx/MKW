using MKW.Core.Serialization.Pgp.Primitives;
using Org.BouncyCastle.Bcpg;
using System.Buffers;

namespace MKW.Core.Serialization.Pgp.Packets
{
    // A Public-Key Encrypted Session Key packet holds the session key used
    // to encrypt a message.  Zero or more Public-Key Encrypted Session Key
    // packets and/or Symmetric-Key Encrypted Session Key packets may
    // precede a Symmetrically Encrypted Data Packet, which holds an
    // encrypted message.  The message is encrypted with the session key,
    // and the session key is itself encrypted and stored in the Encrypted
    // Session Key packet(s).  The Symmetrically Encrypted Data Packet is
    // preceded by one Public-Key Encrypted Session Key packet for each
    // OpenPGP key to which the message is encrypted.  The recipient of the
    // message finds a session key that is encrypted to their public key,
    // decrypts the session key, and then uses the session key to decrypt
    // the message.
    // 
    // The body of this packet consists of:
    // 
    // - A one-octet number giving the version number of the packet type.
    //   The currently defined value for packet version is 3.
    // 
    // - An eight-octet number that gives the Key ID of the public key to
    //   which the session key is encrypted.  If the session key is
    //   encrypted to a subkey, then the Key ID of this subkey is used
    //   here instead of the Key ID of the primary key.
    // 
    // - A one-octet number giving the public-key algorithm used.
    // 
    // - A string of octets that is the encrypted session key.  This
    //   string takes up the remainder of the packet, and its contents are
    //   dependent on the public-key algorithm used.
    public static class PublicKeyEncryptedSessionKeyV3Serializer
    {
        private static readonly PgpVersion version = new PgpVersion(3);
        private static readonly PacketTag tag = PacketTag.PublicKeyEncryptedSession;

        public static PublicKeyEncryptedSessionKeyV3 Deserialize(ArrayBufferReader reader)
        {
            version.ConsumeVersion(reader);

            return new PublicKeyEncryptedSessionKeyV3
            {
                KeyId = reader.ReadBytes(8),
                Tag = (PublicKeyAlgorithmTag)reader.ReadByte(),
                Data = reader.ReadAll(),
            };
        }

        public static void Serialize(IBufferWriter<byte> writer,
                                     PublicKeyEncryptedSessionKeyV3 obj)
        {
            version.Serialize(writer);

            writer.Write(obj.KeyId.Span.Slice(0, 8));
            writer.Write((byte)obj.Tag);
            writer.Write(obj.Data.Span);
        }

        public static void SerializePacket(IBufferWriter<byte> writer,
                                           PublicKeyEncryptedSessionKeyV3 obj,
                                           bool oldFormat)
        {
            ArrayBufferWriter<byte> packetWriter = new ArrayBufferWriter<byte>();

            Serialize(packetWriter, obj);

            PgpPacket packet = new PgpPacket(tag, packetWriter.WrittenMemory);
            PgpPacketSerializer.Serialize(writer, packet, oldFormat);
        }
    }
}
