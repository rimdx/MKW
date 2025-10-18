// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Core.Serialization.OpenPgp.Primitives;
using Org.BouncyCastle.Bcpg;
using System.Buffers;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    // - A 1-octet version number with value 2.
    // - A 1-octet cipher algorithm ID.
    // - A 1-octet AEAD algorithm identifier.
    // - A 1-octet chunk size.
    // - 32 octets of salt. The salt is used to derive the message key and MUST be securely generated (see Section 13.10).
    // - Encrypted data; that is, the output of the selected symmetric key cipher operating in the given AEAD mode.
    // - A final summary authentication tag for the AEAD mode.
    public sealed class SymEncryptedProtectedDataV2Serializer
    {
        private static readonly PgpVersion version = new PgpVersion(2);

        public static SymEncryptedProtectedDataV2 Deserialize(IBufferReader<byte> reader)
        {
            version.ConsumeVersion(reader);

            return new SymEncryptedProtectedDataV2
            {
                CipherAlgorithmTag = (SymmetricKeyAlgorithmTag)reader.ReadByte(),
                AlgorithmTag = (AeadAlgorithmTag)reader.ReadByte(),
                ChunkSize = reader.ReadByte(),
                Salt = reader.ReadBytes(32),
                Data = reader.ReadAll(),
            };
        }

        public static void Serialize(IBufferWriter<byte> writer,
                                     SymEncryptedProtectedDataV2 obj)
        {
            version.Serialize(writer);

            writer.Write((byte)obj.CipherAlgorithmTag);
            writer.Write((byte)obj.AlgorithmTag);
            writer.Write(obj.ChunkSize);

            if (obj.Salt.Length != 32)
            {
                throw new Exception($"Salt must be 32 bytes.");
            }

            writer.Write(obj.Salt.Span);
            writer.Write(obj.Data.Span);
        }
    }
}
