// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Core.Serialization.OpenPgp.Primitives;
using System.Buffers;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    // https://www.rfc-editor.org/rfc/rfc4880#section-5.5.2
    // 
    // A version 4 packet contains:
    // 
    // - A one-octet version number (4).
    // - A four-octet number denoting the time that the key was created.
    // - A one-octet number denoting the public-key algorithm of this key.
    // - A series of multiprecision integers comprising the key material.
    //   This algorithm-specific portion is:
    // 
    //   Algorithm-Specific Fields for RSA public keys:
    //     - multiprecision integer (MPI) of RSA public modulus n;
    //     - MPI of RSA public encryption exponent e.
    // 
    //   Algorithm-Specific Fields for DSA public keys:
    //     - MPI of DSA prime p;
    //     - MPI of DSA group order q (q is a prime divisor of p-1);
    //     - MPI of DSA group generator g;
    //     - MPI of DSA public-key value y (= g**x mod p where x
    //       is secret).
    // 
    //   Algorithm-Specific Fields for Elgamal public keys:
    //     - MPI of Elgamal prime p;
    //     - MPI of Elgamal group generator g;
    //     - MPI of Elgamal public key value y (= g**x mod p where x
    //       is secret).
    public static class PublicKeyPacketV4Serializer
    {
        private static readonly PgpVersion version = new PgpVersion(4);

        public static void Serialize(IBufferWriter<byte> writer,
                                     PublicKeyPacketV4 obj)
        {
            version.Serialize(writer);

            PgpDateTimeSerializer.Serialize(writer, obj.CreatedAt);
            PublicKeyMaterialSerializer.Serialize(writer, obj.PublicKeyMaterial);
        }

        public static PublicKeyPacketV4 Deserialize(IBufferReader reader)
        {
            version.ConsumeVersion(reader);

            return new PublicKeyPacketV4
            {
                CreatedAt = PgpDateTimeSerializer.Deserialize(reader),
                PublicKeyMaterial = PublicKeyMaterialSerializer.Deserialize(reader),
            };
        }
    }
}
