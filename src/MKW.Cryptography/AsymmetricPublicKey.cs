namespace MKW.Cryptography
{
    public sealed record class AsymmetricPublicKey
    {
        public required ReadOnlyMemory<byte> Modulus { get; init; }
        public required ReadOnlyMemory<byte> PublicExponent { get; init; }
    }
}
