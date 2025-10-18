// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Core.Serialization.OpenPgp.Primitives;
using Org.BouncyCastle.Bcpg;
using System.Buffers;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    // https://www.rfc-editor.org/rfc/rfc4880#section-5.5.2
    //
    //   Algorithm-Specific Fields for RSA public keys:
    //     - multiprecision integer (MPI) of RSA public modulus n;
    //     - MPI of RSA public encryption exponent e.
    public static class PublicKeyMaterialRSASerializer
    {
        internal const PublicKeyAlgorithmTag Tag = PublicKeyAlgorithmTag.RsaGeneral;

        public static void Serialize(IBufferWriter<byte> writer,
                                     PublicKeyMaterialRSA obj)
        {
            MPIntegerSerailizer.Serialize(writer, obj.Modulus);
            MPIntegerSerailizer.Serialize(writer, obj.PublicExponent);
        }

        public static PublicKeyMaterialRSA Deserialize(IBufferReader reader)
        {
            return new PublicKeyMaterialRSA
            {
                Modulus = MPIntegerSerailizer.Deserialize(reader),
                PublicExponent = MPIntegerSerailizer.Deserialize(reader),
            };
        }
    }
}
