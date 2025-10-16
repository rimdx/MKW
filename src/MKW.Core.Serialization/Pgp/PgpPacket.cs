using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.Pgp
{
    public sealed record class PgpPacket(
        PacketTag Tag,
        ReadOnlyMemory<byte> EncodedBody
    );
}
