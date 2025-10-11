namespace MKW.Cryptography
{
    public sealed record class AsymmetricAlgorithmConfiguration
    {
        public required AsymmetricAlgorithmEngine Engine { get; init; }
        public required HashAlgorithmEngine HashEngine { get; init; }

        public required int StrengthBits { get; init; }
    }
}
