using MKW.Cryptography;
using Org.BouncyCastle.Security;

namespace MKW.Cryptography.BouncyCastle
{
    internal class BouncyCastleRandomGenerator : IRandomGenerator
    {
        private readonly SecureRandom random;

        public BouncyCastleRandomGenerator()
        {
            random = new SecureRandom();
        }

        public byte[] NextBytes(int length)
        {
            return SecureRandom.GetNextBytes(random, length);
        }
    }
}
