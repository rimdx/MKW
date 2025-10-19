// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using Org.BouncyCastle.Bcpg;
using System.Buffers;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    // https://www.rfc-editor.org/rfc/rfc4880#section-5.11
    public static class UserIdPacketSerializer
    {
        public const PacketTag Tag = PacketTag.UserId;

        public static void Serialize(IBufferWriter<byte> writer,
                                     UserIdPacket obj)
        {
            writer.Write(obj.Content.Span);
        }

        public static UserIdPacket Deserialize(IBufferReader<byte> reader)
        {
            return new UserIdPacket(reader.ReadAll());
        }
    }
}
