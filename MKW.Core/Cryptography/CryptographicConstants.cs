using System.Security.Cryptography;

namespace MKW.Core.Cryptography
{
    public static class CryptographicConstants
    {
        public static class DerivePassword
        {
            public static HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

            // bytes
            public const int SaltSize = 16;

            public const int Iterations = 100_000;

            // bytes
            public const int KeySize = Aes.KeySize;
        }

        public static class Aes
        {
            // bytes
            public const int KeySize = 16;
        }
    }
}
