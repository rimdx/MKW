// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using System.Buffers;

namespace MKW.Cryptography.BouncyCastle
{
    internal static class OpenPgpModificationDetectionPacketSerializer
    {
        internal static ReadOnlySpan<byte> MDPTag => [0xD3, 0x14];

        public static void Serialize(IBufferWriter<byte> writer, OpenPgpModificationDetectionPacket obj)
        {
            writer.Write(obj.Salt.Span.EnsureSize(OpenPgpModificationDetectionPacketConfiguration.BlockSize));
            writer.Write(obj.Salt.Span[^2]);
            writer.Write(obj.Salt.Span[^1]);

            writer.Write(obj.Plaintext.Span);

            writer.Write(MDPTag);
            writer.Write(obj.Checksum.Span.EnsureSize(OpenPgpModificationDetectionPacketConfiguration.Sha1Length));
        }

        public static OpenPgpModificationDetectionPacket Deserialize(IBufferReader<byte> reader)
        {
            int leadBlockSize = OpenPgpModificationDetectionPacketConfiguration.BlockSize + 2;
            int trailBlockSize = MDPTag.Length + OpenPgpModificationDetectionPacketConfiguration.Sha1Length;

            int minPacketSize = leadBlockSize + 0 + trailBlockSize;

            if (reader.RemainingBytes < minPacketSize)
            {
                throw new OpenPgpModificationDetectionPacketCorruptedException("packet is too short");
            }

            ReadOnlyMemory<byte> salt = reader.ReadBytes(OpenPgpModificationDetectionPacketConfiguration.BlockSize);

            if (salt.Span[^2] != reader.ReadByte() ||
                salt.Span[^1] != reader.ReadByte())
            {
                if (true)
                {
                    throw new OpenPgpModificationDetectionPacketCorruptedException("quick check failed");
                }
                else
                {
                    // no-op
                }
            }

            ReadOnlyMemory<byte> plaintext = reader.ReadBytes(reader.RemainingBytes - trailBlockSize);

            ReadOnlyMemory<byte> magic = reader.ReadBytes(MDPTag.Length);

            if (!magic.Span.SequenceEqual(MDPTag))
            {
                throw new OpenPgpModificationDetectionPacketCorruptedException("MDC tag is expected");
            }

            ReadOnlyMemory<byte> checksum = reader.ReadBytes(OpenPgpModificationDetectionPacketConfiguration.Sha1Length);

            return new OpenPgpModificationDetectionPacket
            {
                Salt = salt,
                Plaintext = plaintext,
                Checksum = checksum,
            };
        }
    }
}
