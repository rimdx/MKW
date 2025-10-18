// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Core.Serialization.OpenPgp.Primitives;
using Org.BouncyCastle.Bcpg;
using System.Buffers;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    // The body of this packet consists of:
    // 
    // - A one-octet version number.  The only currently defined version
    //   is 4.
    // 
    // - A one-octet number describing the symmetric algorithm used.
    // 
    // - A string-to-key (S2K) specifier, length as defined above.
    // 
    // - Optionally, the encrypted session key itself, which is decrypted
    //   with the string-to-key object.
    public static class SymEncryptedSessionKeyV4Serializer
    {
        private static readonly PgpVersion version = new PgpVersion(4);

        public static SymEncryptedSessionKeyV4 Deserialize(IBufferReader reader)
        {
            version.ConsumeVersion(reader);

            return new SymEncryptedSessionKeyV4
            {
                SymmetricAlgorithmTag = (SymmetricKeyAlgorithmTag)reader.ReadByte(),
                StringToKey = StringToKeySerializer.Deserialize(reader),
            };
        }

        public static void Serialize(IBufferWriter<byte> writer,
                                     SymEncryptedSessionKeyV4 obj)
        {
            version.Serialize(writer);
            writer.Write((byte)obj.SymmetricAlgorithmTag);
            StringToKeySerializer.Serialize(writer, obj.StringToKey);
        }
    }
}
