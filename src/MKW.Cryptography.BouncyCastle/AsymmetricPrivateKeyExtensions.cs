using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace MKW.Cryptography.BouncyCastle
{
    internal static class AsymmetricPrivateKeyExtensions
    {
        public static RsaPrivateCrtKeyParameters GetParameter(this AsymmetricPrivateKey key)
        {
            return new RsaPrivateCrtKeyParameters(
                new BigInteger(key.Modulus.ToArray()),
                new BigInteger(key.PublicExponent.ToArray()),
                new BigInteger(key.PrivateExponent.ToArray()),
                new BigInteger(key.Prime1.ToArray()),
                new BigInteger(key.Prime2.ToArray()),
                new BigInteger(key.Exponent1.ToArray()),
                new BigInteger(key.Exponent2.ToArray()),
                new BigInteger(key.Coefficient.ToArray())
            );
        }

        public static AsymmetricPrivateKey FromParameter(RsaPrivateCrtKeyParameters key)
        {
            return new AsymmetricPrivateKey
            {
                Modulus = key.Modulus.ToByteArray(),
                PublicExponent = key.PublicExponent.ToByteArray(),
                PrivateExponent = key.Exponent.ToByteArray(),
                Prime1 = key.P.ToByteArray(),
                Prime2 = key.Q.ToByteArray(),
                Exponent1 = key.DP.ToByteArray(),
                Exponent2 = key.DQ.ToByteArray(),
                Coefficient = key.QInv.ToByteArray(),
            };
        }
    }
}
