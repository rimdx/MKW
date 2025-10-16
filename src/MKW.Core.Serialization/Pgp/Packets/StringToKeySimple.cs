using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.Pgp.Packets
{
    public sealed record class StringToKeySimple : StringToKey
    {
        public required HashAlgorithmTag HashAlgorithmTag { get; init; }
    }
}
