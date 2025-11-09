// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Serialization.OpenPgp.MDP;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System.Buffers;

namespace MKW.Cryptography.BouncyCastle
{
    internal sealed class OpenPgpModificationDetectionPacketGenerator
    {
        private readonly IDigest digest;
        private readonly SecureRandom random;

        public OpenPgpModificationDetectionPacketGenerator(IDigest digest, SecureRandom random)
        {
            this.digest = digest;
            this.random = random;
        }

        public OpenPgpModificationDetectionPacket CreatePacket(ReadOnlySpan<byte> plaintext)
        {
            ReadOnlyMemory<byte> salt = SecureRandom.GetNextBytes(random, OpenPgpModificationDetectionPacketConfiguration.BlockSize);
            ReadOnlyMemory<byte> checksum = GetChecksum(salt.Span, plaintext);

            return new OpenPgpModificationDetectionPacket
            {
                Salt = salt,
                Plaintext = plaintext.ToArray(), // todo: prevent copy?
                Checksum = checksum,
            };
        }

        public ReadOnlyMemory<byte> OpenPlaintext(OpenPgpModificationDetectionPacket packet)
        {
            ReadOnlyMemory<byte> actualChecksum = GetChecksum(packet.Salt.Span,
                                                              packet.Plaintext.Span);

            if (packet.Checksum.Span.SequenceEqual(actualChecksum.Span))
            {
                return packet.Plaintext;
            }
            else
            {
                throw new OpenPgpModificationDetectionPacketChecksumMismatchException();
            }
        }

        private ReadOnlyMemory<byte> GetChecksum(ReadOnlySpan<byte> salt, ReadOnlySpan<byte> plaintext)
        {
            byte[] checksum = new byte[digest.GetDigestSize()];

            digest.Reset();

            ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();
            OpenPgpModificationDetectionPacketSerializer.SerializeChecksum(writer, salt, plaintext);

            digest.BlockUpdate(writer.WrittenSpan);

            digest.DoFinal(checksum, 0);

            return checksum;
        }
    }
}
