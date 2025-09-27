using System.Security.Cryptography;

namespace MKW.Cryptography.System
{
    internal sealed class SystemRandomGenerator : IRandomGenerator
    {
        public SystemRandomGenerator()
        {
        }

        public byte[] NextBytes(int length)
        {
            return RandomNumberGenerator.GetBytes(length);
        }
    }
}