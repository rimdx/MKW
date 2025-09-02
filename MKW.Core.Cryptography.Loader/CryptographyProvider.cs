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

        public static ICryptographyProvider Create(string name) => name switch
        {
            BouncyCastleLoader.Name => BouncyCastleLoader.Create(),
            SystemCryptographyLoader.Name => BouncyCastleLoader.Create(),
            _ => throw new NotSupportedException(),
        };
    }
}
