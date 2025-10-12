namespace MKW.Cryptography
{
    public static class CommonCryptographyAlgorithms
    {
        public static readonly SymmetricAlgorithmConfiguration Aes128Gcm =
            new SymmetricAlgorithmConfiguration
            {
                Engine = SymmetricAlgorithmEngine.AesGcm,
                KeySizeBits = 128,
                IVSizeBits = 128,
            };

        public static readonly AsymmetricAlgorithmConfiguration Rsa2048 =
            new AsymmetricAlgorithmConfiguration
            {
                Engine = AsymmetricAlgorithmEngine.Rsa,
                HashEngine = HashAlgorithmEngine.Sha256,
                StrengthBits = 2048,
            };

        public static readonly PasswordDerivationConfiguration Pbkdf2 =
            new PasswordDerivationConfiguration
            {
                Engine = PasswordDerivationEngine.Pbkdf2,
                HashEngine = HashAlgorithmEngine.Sha256,
                Iterations = 100_000,
                KeySizeBits = 128,
                SaltSizeBits = 128,
            };
    }
}
