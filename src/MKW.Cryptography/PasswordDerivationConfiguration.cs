using System.Security.Cryptography;

namespace MKW.Cryptography
{
    public sealed record class PasswordDerivationConfiguration
    {
        public required PasswordDerivationEngine Engine { get; init; }

        public required HashAlgorithmEngine HashEngine { get; init; }

        public required int SaltSizeBits { get; init; }
        public required int KeySizeBits { get; init; }

        public required int Iterations { get; init; }
    }
}
