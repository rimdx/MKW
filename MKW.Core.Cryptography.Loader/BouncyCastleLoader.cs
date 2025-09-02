using MKW.Core.Cryptography.BouncyCastle;

namespace MKW.Core.Cryptography.Loader
{
    public static class BouncyCastleLoader
    {
        public static bool Supported => true;

        public static ICryptographyProvider Create()
        {
            return new BouncyCastleCryptographyProvider();
        }
    }
}
