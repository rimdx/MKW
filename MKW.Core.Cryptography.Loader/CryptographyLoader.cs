namespace MKW.Core.Cryptography.Loader
{
    public static class CryptographyLoader
    {
        public static ICryptographyProvider GetProvider()
        {
            if (SystemCryptographyLoader.Supported)
            {
                return SystemCryptographyLoader.GetProvider();
            }
            else if (BouncyCastleLoader.Supported)
            {
                return BouncyCastleLoader.GetProvider();
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static ICryptographyProvider GetProvider(string name) => name switch
        {
            BouncyCastleLoader.Name => BouncyCastleLoader.GetProvider(),
            SystemCryptographyLoader.Name => BouncyCastleLoader.GetProvider(),
            _ => throw new NotSupportedException(),
        };
    }
}
