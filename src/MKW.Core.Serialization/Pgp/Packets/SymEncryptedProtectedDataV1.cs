namespace MKW.Core.Serialization.Pgp.Packets
{
    public sealed record class SymEncryptedProtectedDataV1
    {
        public required ReadOnlyMemory<byte> Data { get; init; }
    }
}
