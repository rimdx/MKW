namespace MKW.Cryptography
{
    public sealed record class SymmetricAlgorithmConfiguration
    {
        public required SymmetricAlgorithmEngine Engine { get; init; }
        public required int KeySizeBits { get; init; }
        public required int IVSizeBits { get; init; }
    }
}
