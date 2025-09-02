namespace MKW.Core.Cryptography.Loader
{
    public static class CryptographyLoader
    {
        public static ICryptographyProvider GetProvider()
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

        public static ICryptographyProvider GetProvider(string name) => name switch
        {
            BouncyCastleLoader.Name => BouncyCastleLoader.Create(),
            SystemCryptographyLoader.Name => BouncyCastleLoader.Create(),
            _ => throw new NotSupportedException(),
        };
    }
}
