using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.Pgp.Packets
{
    public sealed record class PublicKeyEncryptedSessionKeyV3
    {
        public required ReadOnlyMemory<byte> KeyId { get; init; }
        public required PublicKeyAlgorithmTag Tag { get; init; }
        public required ReadOnlyMemory<byte> Data { get; init; }
    }
}
