using MKW.Cryptography;

namespace MKW.Cryptography.Loader
{
    public class SystemCryptographyLoader
    {
        public const string Name = "System.Security";

#if NETFRAMEWORK
        public const bool Supported = false;

        public static ICryptographyProvider GetProvider()
        {
            throw new NotSupportedException();
        }
#else
        public const bool Supported = true;

        public static ICryptographyProvider GetProvider()
        {
            return new System.CryptographyProvider();
        }
#endif
    }
}
