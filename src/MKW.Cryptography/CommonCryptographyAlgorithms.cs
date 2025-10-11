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
    }
}
