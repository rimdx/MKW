using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.Pgp.Packets
{
    public sealed record class SymEncryptedProtectedDataV2
    {
        public required SymmetricKeyAlgorithmTag CipherAlgorithmTag { get; init; }
        public required AeadAlgorithmTag AlgorithmTag { get; init; }
        public required byte ChunkSize { get; init; }
        public required ReadOnlyMemory<byte> Data { get; init; }
        public required ReadOnlyMemory<byte> Salt { get; init; }
    }
}
