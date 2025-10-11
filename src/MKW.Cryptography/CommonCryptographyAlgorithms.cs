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
    }
}
