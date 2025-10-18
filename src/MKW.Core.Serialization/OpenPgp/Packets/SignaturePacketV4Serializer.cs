// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using MKW.Core.Serialization.OpenPgp.Primitives;
using Org.BouncyCastle.Bcpg;
using System.Buffers;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    // https://www.rfc-editor.org/rfc/rfc4880#section-5.2.3
    public static class SignaturePacketV4Serializer
    {
        private static readonly PgpVersion version = new PgpVersion(4);

        public static void Serialize(IBufferWriter<byte> writer,
                                     SignaturePacketV4 obj)
        {
            version.Serialize(writer);

            writer.Write((byte)obj.Type);
            writer.Write((byte)obj.PublicKeyAlgorithm);
            writer.Write((byte)obj.HashAlgorithm);

            int len = obj.RawData.Length & 0xFFFF;
            writer.Write((byte)((len >> 8) & 0xFF));
            writer.Write((byte)((len >> 0) & 0xFF));

            writer.Write(obj.RawData.Span);
            writer.Write(obj.Signature.Span);
        }

        public static SignaturePacketV4 Deserialize(IBufferReader reader)
        {
            version.ConsumeVersion(reader);

            SignatureTypeTag type = (SignatureTypeTag)reader.ReadByte();
            PublicKeyAlgorithmTag publicKeyAlgorithm = (PublicKeyAlgorithmTag)reader.ReadByte();
            HashAlgorithmTag hashAlgorithm = (HashAlgorithmTag)reader.ReadByte();

            byte len0 = reader.ReadByte();
            byte len1 = reader.ReadByte();
            int len = len0 << 8 | len1;

            ReadOnlyMemory<byte> data = reader.ReadBytes(len);
            ReadOnlyMemory<byte> signature = reader.ReadAll();

            return new SignaturePacketV4
            {
                Type = type,
                PublicKeyAlgorithm = publicKeyAlgorithm,
                HashAlgorithm = hashAlgorithm,
                RawData = data,
                Signature = signature,
            };
        }
    }
}
