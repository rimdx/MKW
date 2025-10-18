// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Core.Serialization.OpenPgp.Primitives;
using Org.BouncyCastle.Bcpg;
using System.Buffers;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    // - A one-octet version number.  The only currently defined value is 1.
    //
    // - Encrypted data, the output of the selected symmetric-key cipher
    //   operating in Cipher Feedback mode with shift amount equal to the
    //   block size of the cipher (CFB-n where n is the block size).
    public static class SymEncryptedProtectedDataV1Serializer
    {
        private static readonly PgpVersion version = new PgpVersion(1);
        private static readonly PacketTag tag = PacketTag.SymmetricEncryptedIntegrityProtected;

        public static void Serialize(IBufferWriter<byte> writer,
                                     SymEncryptedProtectedDataV1 obj)
        {
            version.Serialize(writer);
            writer.Write(obj.Data.Span);
        }

        public static void SerializePacket(IBufferWriter<byte> writer,
                                           SymEncryptedProtectedDataV1 obj,
                                           bool oldFormat)
        {
            ArrayBufferWriter<byte> packetWriter = new ArrayBufferWriter<byte>();

            Serialize(packetWriter, obj);

            PgpPacket packet = new PgpPacket(tag, packetWriter.WrittenMemory);
            PgpPacketSerializer.Serialize(writer, packet, oldFormat);
        }

        public static SymEncryptedProtectedDataV1 Deserialize(IBufferReader<byte> reader)
        {
            version.ConsumeVersion(reader);

            ReadOnlyMemory<byte> data = reader.ReadAll();

            return new SymEncryptedProtectedDataV1
            {
                Data = data,
            };
        }
    }
}
