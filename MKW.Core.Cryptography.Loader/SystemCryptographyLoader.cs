namespace MKW.Core.Cryptography.Loader
{
    public class SystemCryptographyLoader
    {
#if NETFRAMEWORK
        public static bool Supported => false;

        public static ICryptographyProvider Create()
        {
            throw new NotSupportedException();
        }
#else
        public static bool Supported => true;

        public static ICryptographyProvider Create()
        {
            return new System.CryptographyProvider();
        }
#endif
    }
}
