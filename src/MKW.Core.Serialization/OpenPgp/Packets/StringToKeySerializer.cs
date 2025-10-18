// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using Org.BouncyCastle.Bcpg;
using System.Buffers;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public static class StringToKeySerializer
    {
        public static StringToKey Deserialize(ArrayBufferReader reader)
        {
            StringToKeyTag tag = (StringToKeyTag)reader.ReadByte();

            if (tag == StringToKeyTag.Simple)
            {
                // Octet 0:        0x00
                // Octet 1:        hash algorithm

                return new StringToKeySimple
                {
                    Tag = tag,
                    HashAlgorithmTag = (HashAlgorithmTag)reader.ReadByte(),
                };
            }
            else if (tag == StringToKeyTag.Salted)
            {
                // Octet 0:        0x01
                // Octet 1:        hash algorithm
                // Octets 2-9:     8-octet salt value

                return new StringToKeySalted
                {
                    Tag = tag,
                    HashAlgorithmTag = (HashAlgorithmTag)reader.ReadByte(),
                    Salt = reader.ReadBytes(8),
                };
            }
            else if (tag == StringToKeyTag.IteratedSalted)
            {
                // Octet  0:        0x03
                // Octet  1:        hash algorithm
                // Octets 2-9:      8-octet salt value
                // Octet  10:       count, a one-octet, coded value

                return new StringToKeySaltedIterated
                {
                    Tag = tag,
                    HashAlgorithmTag = (HashAlgorithmTag)reader.ReadByte(),
                    Salt = reader.ReadBytes(8),
                    Count = reader.ReadByte(),
                };
            }
            else
            {
                throw new Exception($"Unknown S2K algorithm: {tag}");
            }
        }

        public static void Serialize(IBufferWriter<byte> writer,
                                     StringToKey obj)
        {
            writer.Write((byte)obj.Tag);

            if (obj is StringToKeySimple simple)
            {
                writer.Write((byte)simple.HashAlgorithmTag);
            }
            else if (obj is StringToKeySalted salted)
            {
                writer.Write((byte)salted.HashAlgorithmTag);
                writer.Write(salted.Salt.Span.Slice(0, 8));
            }
            else if (obj is StringToKeySaltedIterated saltedIterated)
            {
                writer.Write((byte)saltedIterated.HashAlgorithmTag);
                writer.Write(saltedIterated.Salt.Span.Slice(0, 8));
                writer.Write(saltedIterated.Count);
            }
            else
            {
                throw new Exception($"Unknown S2K algorithm: {obj}");
            }
        }
    }
}
