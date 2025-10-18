// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace MKW.Cryptography.BouncyCastle
{
    internal static class AsymmetricPublicKeyExtensions
    {
        public static RsaKeyParameters GetParameter(this AsymmetricPublicKey key)
        {
            return new RsaKeyParameters(
                false,
                new BigInteger(key.Modulus.ToArray()),
                new BigInteger(key.PublicExponent.ToArray())
            );
        }

        public static AsymmetricPublicKey FromParameter(RsaKeyParameters key)
        {
            return new AsymmetricPublicKey
            {
                Modulus = key.Modulus.ToByteArray(),
                PublicExponent = key.Exponent.ToByteArray(),
            };
        }
    }
}
