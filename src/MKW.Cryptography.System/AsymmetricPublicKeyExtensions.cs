using System.Security.Cryptography;

namespace MKW.Cryptography.System
{
    internal static class AsymmetricPublicKeyExtensions
    {
        public static RSAParameters GetParameter(this AsymmetricPublicKey key)
        {
            return new RSAParameters
            {
                Modulus = key.Modulus.ToArray(),
                Exponent = key.PublicExponent.ToArray(),
            };
        }

        public static AsymmetricPublicKey FromParameter(RSAParameters key)
        {
            return new AsymmetricPublicKey
            {
                Modulus = key.Modulus,
                PublicExponent = key.Exponent,
            };
        }
    }
}
