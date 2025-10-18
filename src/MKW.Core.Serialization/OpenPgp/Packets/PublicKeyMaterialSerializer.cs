// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using Org.BouncyCastle.Bcpg;
using System.Buffers;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    // https://www.rfc-editor.org/rfc/rfc4880#section-5.5.2
    public static class PublicKeyMaterialSerializer
    {
        public static void Serialize(IBufferWriter<byte> writer,
                                     PublicKeyMaterial obj)
        {
            if (obj is PublicKeyMaterialRSA rsa)
            {
                writer.Write((byte)PublicKeyMaterialRSASerializer.Tag);
                PublicKeyMaterialRSASerializer.Serialize(writer, rsa);
            }
            else
            {
                throw new Exception("Unsupported PublicKeyMaterial.");
            }
        }

        public static PublicKeyMaterial Deserialize(ArrayBufferReader reader)
        {
            PublicKeyAlgorithmTag tag = (PublicKeyAlgorithmTag)reader.ReadByte();

            if (tag == PublicKeyMaterialRSASerializer.Tag)
            {
                return PublicKeyMaterialRSASerializer.Deserialize(reader);
            }
            else
            {
                throw new Exception("Unsupported PublicKeyMaterial.");
            }
        }
    }
}
