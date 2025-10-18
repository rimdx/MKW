// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using System.Buffers;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public static class SignaturePacketV4BodySerializer
    {
        public static void Serialize(IBufferWriter<byte> writer,
                                     SignaturePacketV4Body obj)
        {
            foreach (SignatureSubpacketV4 packet in obj.Subpackets)
            {
                SignatureSubpacketV4Serializer.Serialize(writer, packet);
            }
        }

        public static SignaturePacketV4Body Deserialize(IBufferReader<byte> reader)
        {
            List<SignatureSubpacketV4> subpackets = [];

            while (reader.RemainingBytes > 0)
            {
                subpackets.Add(SignatureSubpacketV4Serializer.Deserialize(reader));
            }

            return new SignaturePacketV4Body
            {
                Subpackets = subpackets,
            };
        }
    }
}
