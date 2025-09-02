namespace MKW.Core.Cryptography.Loader
{
    public class SystemCryptographyLoader
    {
        public const string Name = "System.Security";

#if NETFRAMEWORK
        public const bool Supported = false;

        public static ICryptographyProvider Create()
        {
            throw new NotSupportedException();
        }
#else
        public const bool Supported = true;

        public static ICryptographyProvider Create()
        {
            return new System.CryptographyProvider();
        }
#endif
    }
}
