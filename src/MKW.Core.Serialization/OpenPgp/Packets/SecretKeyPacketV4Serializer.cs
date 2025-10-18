// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using System.Buffers;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    // https://www.rfc-editor.org/rfc/rfc4880#section-5.5.3
    // 
    // The Secret-Key and Secret-Subkey packets contain all the data of the
    // Public-Key and Public-Subkey packets, with additional algorithm-
    // specific secret-key data appended, usually in encrypted form.
    // 
    // The packet contains:
    // 
    // - A Public-Key or Public-Subkey packet, as described above.
    // 
    // - One octet indicating string-to-key usage conventions.  Zero
    //   indicates that the secret-key data is not encrypted.  255 or 254
    //   indicates that a string-to-key specifier is being given.  Any
    //   other value is a symmetric-key encryption algorithm identifier.
    // 
    // [...strip...]
    // 
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
    public static class SecretKeyPacketV4Serializer
    {
        public static void Serialize(IBufferWriter<byte> writer,
                                     SecretKeyPacketV4 obj)
        {
            PublicKeyPacketV4Serializer.Serialize(writer, obj.PublicKey);
            SecretKeyStringToKeySerializer.Serialize(writer, obj.StringToKey);
            writer.Write(obj.SecretKeyData.Span);
        }

        public static SecretKeyPacketV4 Deserialize(ArrayBufferReader reader)
        {
            return new SecretKeyPacketV4
            {
                PublicKey = PublicKeyPacketV4Serializer.Deserialize(reader),
                StringToKey = SecretKeyStringToKeySerializer.Deserialize(reader),
                SecretKeyData = reader.ReadAll(),
            };
        }
    }
}
