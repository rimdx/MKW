// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Core.Serialization.OpenPgp.Primitives;
using Org.BouncyCastle.Math;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    // Algorithm-Specific Fields for RSA secret keys:
    // - multiprecision integer (MPI) of RSA secret exponent d.
    // - MPI of RSA secret prime value p.
    // - MPI of RSA secret prime value q (p < q).
    // - MPI of u, the multiplicative inverse of p, mod q.
    //
    // Algorithm-Specific Fields for DSA secret keys:
    // - MPI of DSA secret exponent x.
    //
    // Algorithm-Specific Fields for Elgamal secret keys:
    // - MPI of Elgamal secret exponent x.
    public static class SecretKeyMaterialRSASerializer
    {
        public static SecretKeyMaterialRSA Deserialize(IBufferReader<byte> reader)
        {
            BigInteger d = MPIntegerSerailizer.Deserialize(reader);

            BigInteger p = MPIntegerSerailizer.Deserialize(reader);
            BigInteger q = MPIntegerSerailizer.Deserialize(reader);

            BigInteger u = MPIntegerSerailizer.Deserialize(reader);

            return new SecretKeyMaterialRSA
            {
                PrivateExponent = d,
                PrimeP = p,
                PrimeQ = q,
                Inverse = u,
            };
        }
    }
}
