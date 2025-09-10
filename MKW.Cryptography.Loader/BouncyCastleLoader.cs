using MKW.Cryptography;
using MKW.Cryptography.BouncyCastle;

namespace MKW.Cryptography.Loader
{
    public static class BouncyCastleLoader
    {
        public const string Name = "BouncyCastle.Cryptography";

        public const bool Supported = true;

        public static ICryptographyProvider GetProvider()
        {
            return new BouncyCastleCryptographyProvider();
        }
    }
}
