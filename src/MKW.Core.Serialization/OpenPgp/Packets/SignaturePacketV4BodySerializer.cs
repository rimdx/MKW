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
            foreach (SignatureSubpacket packet in obj.Subpackets)
            {
                SignatureSubpacketSerializer.Serialize(writer, packet);
            }
        }

        public static SignaturePacketV4Body Deserialize(IBufferReader reader)
        {
            List<SignatureSubpacket> subpackets = [];

            while (reader.RemainingBytes > 0)
            {
                subpackets.Add(SignatureSubpacketSerializer.Deserialize(reader));
            }

            return new SignaturePacketV4Body
            {
                Subpackets = subpackets,
            };
        }
    }
}
