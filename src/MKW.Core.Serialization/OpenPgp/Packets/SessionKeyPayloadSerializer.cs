// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using Org.BouncyCastle.Bcpg;
using System.Buffers;
using System.Buffers.Binary;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    // The value "m" in the above formulas is derived from the session key
    // as follows.First, the session key is prefixed with a one-octet
    // algorithm identifier that specifies the symmetric encryption
    // algorithm used to encrypt the following Symmetrically Encrypted Data
    // Packet.  Then a two-octet checksum is appended, which is equal to the
    // sum of the preceding session key octets, not including the algorithm
    // identifier, modulo 65536.  This value is then encoded as described in
    // PKCS#1 block encoding EME-PKCS1-v1_5 in Section 7.2.1 of [RFC3447] to
    // form the "m" value used in the formulas above.See Section 13.1 of
    // this document for notes on OpenPGP's use of PKCS#1.
    public static class SessionKeyPayloadSerializer
    {
        private const int checksumSize = 2;

        public static void Serialize(IBufferWriter<byte> writer,
                                     SessionKeyPayload obj)
        {
            writer.Write((byte)obj.SymmetricAlgorithm);
            writer.Write(obj.KeyBytes.Span);

            Span<byte> buf = writer.GetSpan(checksumSize);
            BinaryPrimitives.WriteUInt16BigEndian(buf, obj.Checksum);
        }

        public static SessionKeyPayload Deserialize(IBufferReader<byte> reader)
        {
            return new SessionKeyPayload
            {
                SymmetricAlgorithm = (SymmetricKeyAlgorithmTag)reader.ReadByte(),
                KeyBytes = reader.ReadBytes(reader.RemainingBytes - checksumSize),
                Checksum = BinaryPrimitives.ReadUInt16BigEndian(reader.ReadBytes(checksumSize).Span),
            };
        }
    }
}
