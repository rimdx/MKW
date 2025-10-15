using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.Pgp
{
    public abstract class PgpPacket : PgpObject
    {
        public PacketTag Tag { get; }

        public PgpPacket(PacketTag packetTag)
        {
            Tag = packetTag;
        }
    }
}
