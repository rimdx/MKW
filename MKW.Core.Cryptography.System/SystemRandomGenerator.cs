using System.Security.Cryptography;

namespace MKW.Core.Cryptography.System
{
    internal class SystemRandomGenerator : IRandomGenerator
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