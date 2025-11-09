// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using System.Buffers;

namespace MKW.Cryptography.BouncyCastle
{
    internal static class OpenPgpModificationDetectionPacketSerializer
    {
        public static void Serialize(IBufferWriter<byte> writer, OpenPgpModificationDetectionPacket obj)
        {
            writer.Write(obj.Salt.Span.EnsureSize(OpenPgpModificationDetectionPacketConfiguration.BlockSize));
            writer.Write(obj.Salt.Span[^2]);
            writer.Write(obj.Salt.Span[^1]);

            writer.Write(obj.Plaintext.Span);

            writer.Write(OpenPgpModificationDetectionPacketConfiguration.MDPTag);
            writer.Write(obj.Checksum.Span.EnsureSize(OpenPgpModificationDetectionPacketConfiguration.Sha1Length));
        }

        public static OpenPgpModificationDetectionPacket Deserialize(IBufferReader<byte> reader)
        {
            int leadBlockSize = OpenPgpModificationDetectionPacketConfiguration.BlockSize + 2;

            int trailBlockSize =
                OpenPgpModificationDetectionPacketConfiguration.MDPTag.Length +
                OpenPgpModificationDetectionPacketConfiguration.Sha1Length;

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

            ReadOnlyMemory<byte> magic = reader.ReadBytes(OpenPgpModificationDetectionPacketConfiguration.MDPTag.Length);

            if (!magic.Span.SequenceEqual(OpenPgpModificationDetectionPacketConfiguration.MDPTag))
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

        public static void SerializeChecksum(IBufferWriter<byte> writer,
                                             ReadOnlySpan<byte> salt,
                                             ReadOnlySpan<byte> plaintext)
        {
            Span<byte> prefix = [
                .. salt.EnsureSize(OpenPgpModificationDetectionPacketConfiguration.BlockSize),
                salt[^2],
                salt[^1],
            ];

            writer.Write(prefix);
            writer.Write(plaintext);
            writer.Write(OpenPgpModificationDetectionPacketConfiguration.MDPTag);
        }
    }
}
