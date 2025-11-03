// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using System.Buffers;
using System.Buffers.Binary;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    // https://www.rfc-editor.org/rfc/rfc4880#section-5.2.4
    // All signatures are formed by producing a hash over the signature
    // data, and then using the resulting hash in the signature algorithm.
    public static class CertificationSignatureSerializer
    {
        public static void Serialize(IBufferWriter<byte> writer,
                                     ReadOnlySpan<byte> publicKeyBytes,
                                     UserIdPacket userId)
        {
            {
                // When a signature is made over a key, the hash data starts with the
                // octet 0x99, followed by a two-octet length of the key, and then body
                // of the key packet.  (Note that this is an old-style packet header for
                // a key packet with two-octet length.)  A subkey binding signature
                // (type 0x18) or primary key binding signature (type 0x19) then hashes
                // the subkey using the same format as the main key (also using 0x99 as
                // the first octet).  Key revocation signatures (types 0x20 and 0x28)
                // hash only the key being revoked.

                int keyLen = publicKeyBytes.Length;

                Span<byte> buf = [
                    0x99, // mask
                    (byte)((keyLen >> 8) % 0xFF),
                    (byte)(keyLen % 0xFF),
                ];

                writer.Write(buf);
                writer.Write(publicKeyBytes);
            }

            {
                // A certification signature (type 0x10 through 0x13) hashes the User
                // ID being bound to the key into the hash context after the above
                // data.  A V3 certification hashes the contents of the User ID or
                // attribute packet packet, without any header.  A V4 certification
                // hashes the constant 0xB4 for User ID certifications or the constant
                // 0xD1 for User Attribute certifications, followed by a four-octet
                // number giving the length of the User ID or User Attribute data, and
                // then the User ID or User Attribute data.

                ArrayBufferWriter<byte> userIdWriter = new ArrayBufferWriter<byte>();
                UserIdPacketSerializer.Serialize(userIdWriter, userId);

                int userIdLen = userIdWriter.WrittenCount;

                Span<byte> buf = new byte[5];
                buf[0] = 0xB4;
                BinaryPrimitives.WriteInt32BigEndian(buf.Slice(1), userIdLen);

                writer.Write(buf);
                writer.Write(userIdWriter.WrittenSpan);
            }
        }
    }
}
