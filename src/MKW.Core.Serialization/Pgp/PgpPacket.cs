using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.Pgp
{
    public abstract class PgpPacket
    {
        public abstract PacketTag Tag { get; }

        public abstract void Encode(PgpOutputStream stream);
    }
}
