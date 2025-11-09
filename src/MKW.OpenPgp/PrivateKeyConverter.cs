// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Serialization.OpenPgp.Packets;
using MKW.Cryptography;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace MKW.OpenPgp
{
    // If there is anyone on Earth who can understand this code -- we
    // probably are safe as a society.
    internal static class PrivateKeyConverter
    {
        public static AsymmetricPrivateKey OpenPrivateKey(PublicKeyMaterialRSA pubkeyMaterial,
                                                          SecretKeyMaterialRSA seckeyMaterial)
        {
            BigInteger exponent1 = seckeyMaterial.PrivateExponent.Remainder(seckeyMaterial.PrimeP.Subtract(BigInteger.One));
            BigInteger exponent2 = seckeyMaterial.PrivateExponent.Remainder(seckeyMaterial.PrimeQ.Subtract(BigInteger.One));

            BigInteger coefficient = BigIntegers.ModOddInverse(seckeyMaterial.PrimeP, seckeyMaterial.PrimeQ);

            return new AsymmetricPrivateKey
            {
                Modulus = pubkeyMaterial.Modulus.ToByteArray(),
                PublicExponent = pubkeyMaterial.PublicExponent.ToByteArray(),
                PrivateExponent = seckeyMaterial.PrivateExponent.ToByteArray(),
                Prime1 = seckeyMaterial.PrimeP.ToByteArray(),
                Prime2 = seckeyMaterial.PrimeQ.ToByteArray(),
                Exponent1 = exponent1.ToByteArray(),
                Exponent2 = exponent2.ToByteArray(),
                Coefficient = coefficient.ToByteArray(),
            };
        }

        public static SecretKeyMaterialRSA ExportPrivateKey(AsymmetricPrivateKey key)
        {
            BigInteger d = new BigInteger(key.PrivateExponent.ToArray()); 
            BigInteger p = new BigInteger(key.Prime1.ToArray());
            BigInteger q = new BigInteger(key.Prime2.ToArray());
            BigInteger u = BigIntegers.ModOddInverse(p, q);

            return new SecretKeyMaterialRSA
            {
                PrivateExponent = u,
                PrimeP = p,
                PrimeQ = q,
                Inverse = u,
            };
        }
    }
}
