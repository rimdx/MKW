namespace MKW.Core.Cryptography.Loader
{
    public static class CryptographyProvider
    {
        public static ICryptographyProvider Create()
        {
            if (SystemCryptographyLoader.Supported)
            {
                return SystemCryptographyLoader.Create();
            }
            else if (BouncyCastleLoader.Supported)
            {
                return BouncyCastleLoader.Create();
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
