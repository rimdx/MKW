using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.Pgp.Packets
{
    public sealed record class StringToKeySalted : StringToKey
    {
        public required HashAlgorithmTag HashAlgorithmTag { get; init; }
        public required ReadOnlyMemory<byte> Salt { get; init; }
    }
}
