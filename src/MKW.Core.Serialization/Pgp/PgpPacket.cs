using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.Pgp
{
    public abstract class PgpPacket : PgpObject
    {
        public abstract PacketTag Tag { get; }
    }
}
