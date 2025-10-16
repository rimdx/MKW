using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.Pgp.Packets
{
    public sealed record class SymEncryptedSessionKeyV4
    {
        public required SymmetricKeyAlgorithmTag SymmetricAlgorithmTag { get; init; }
        public required StringToKey StringToKey { get; init; }
    }
}
