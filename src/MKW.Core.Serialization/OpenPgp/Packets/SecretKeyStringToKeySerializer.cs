// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using Org.BouncyCastle.Bcpg;
using System.Buffers;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    // https://www.rfc-editor.org/rfc/rfc4880#section-5.5.3
    // 
    // - One octet indicating string-to-key usage conventions.  Zero
    //   indicates that the secret-key data is not encrypted.  255 or 254
    //   indicates that a string-to-key specifier is being given.  Any
    //   other value is a symmetric-key encryption algorithm identifier.
    // 
    // - [Optional] If string-to-key usage octet was 255 or 254, a one-
    //   octet symmetric encryption algorithm.
    // 
    // - [Optional] If string-to-key usage octet was 255 or 254, a
    //   string-to-key specifier.  The length of the string-to-key
    //   specifier is implied by its type, as described above.
    // 
    // - [Optional] If secret data is encrypted (string-to-key usage octet
    //   not zero), an Initial Vector (IV) of the same length as the
    //   cipher's block size.
    // 
    // - Plain or encrypted multiprecision integers comprising the secret
    //   key data.  These algorithm-specific fields are as described
    //   below.
    // 
    // - If the string-to-key usage octet is zero or 255, then a two-octet
    //   checksum of the plaintext of the algorithm-specific portion (sum
    //   of all octets, mod 65536).  If the string-to-key usage octet was
    //   254, then a 20-octet SHA-1 hash of the plaintext of the
    //   algorithm-specific portion.  This checksum or hash is encrypted
    //   together with the algorithm-specific fields (if string-to-key
    //   usage octet is not zero).  Note that for all other values, a
    //   two-octet checksum is required.
    public static class SecretKeyStringToKeySerializer
    {
        private const byte UsageNotEncrypted = 0;
        private const byte UsageStringToKeyWithCryptoChecksum = 254;
        private const byte UsageStringToKeyWithSimpleChecksum = 255;

        public static void Serialize(IBufferWriter<byte> writer,
                                     StringToKey obj)
        {
            if (obj is StringToKeyNone)
            {
                writer.Write(UsageNotEncrypted);
            }
            else
            {
                // TODO:
                writer.Write(UsageStringToKeyWithCryptoChecksum);
                StringToKeySerializer.Serialize(writer, obj);
            }
        }

        public static StringToKey Deserialize(ArrayBufferReader reader)
        {
            byte usage = reader.ReadByte();

            if (usage == UsageStringToKeyWithCryptoChecksum ||
                usage == UsageStringToKeyWithSimpleChecksum)
            {
                SymmetricKeyAlgorithmTag symmetricAlgorithm = (SymmetricKeyAlgorithmTag)reader.ReadByte();
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
                    return new StringToKeyNone
                    {
                        Tag = StringToKeyTag.None,
                    };
                }
            }
            else if (usage == UsageNotEncrypted)
            {
                return new StringToKeyNone
                {
                    Tag = StringToKeyTag.None,
                };
            }
            else
            {
                throw new Exception("Unsupported string-to-key usage.");
            }
        }
    }
}
