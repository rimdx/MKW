// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

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
