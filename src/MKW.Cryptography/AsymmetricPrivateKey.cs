namespace MKW.Cryptography
{
    public sealed record class AsymmetricPrivateKey
    {
        public required ReadOnlyMemory<byte> Modulus { get; init; }
        public required ReadOnlyMemory<byte> PublicExponent { get; init; }
        public required ReadOnlyMemory<byte> PrivateExponent { get; init; }
        public required ReadOnlyMemory<byte> Prime1 { get; init; }
        public required ReadOnlyMemory<byte> Prime2 { get; init; }
        public required ReadOnlyMemory<byte> Exponent1 { get; init; }
        public required ReadOnlyMemory<byte> Exponent2 { get; init; }
        public required ReadOnlyMemory<byte> Coefficient { get; init; }

        public AsymmetricPublicKey GetPublicKey()
        {
            return new AsymmetricPublicKey
            {
                Modulus = Modulus,
                PublicExponent = PublicExponent,
            };
        }
    }
}
