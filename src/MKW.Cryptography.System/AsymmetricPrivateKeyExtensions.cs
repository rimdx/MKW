// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Security.Cryptography;

namespace MKW.Cryptography.System
{
    internal static class AsymmetricPrivateKeyExtensions
    {
        public static RSAParameters GetParameter(this AsymmetricPrivateKey key)
        {
            return new RSAParameters
            {
                Modulus = key.Modulus.ToArray(),
                Exponent = key.PublicExponent.ToArray(),
                D = key.PrivateExponent.ToArray(),
                P = key.Prime1.ToArray(),
                Q = key.Prime2.ToArray(),
                DP = key.Exponent1.ToArray(),
                DQ = key.Exponent2.ToArray(),
                InverseQ = key.Coefficient.ToArray(),
            };
        }

        public static AsymmetricPrivateKey FromParameter(RSAParameters key)
        {
            return new AsymmetricPrivateKey
            {
                Modulus = key.Modulus,
                PublicExponent = key.Exponent,
                PrivateExponent = key.D,
                Prime1 = key.P,
                Prime2 = key.Q,
                Exponent1 = key.DP,
                Exponent2 = key.DQ,
                Coefficient = key.InverseQ,
            };
        }
    }
}
