// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public static class PgpPacketReader
    {
        public static IEnumerable<PgpPacketBody> ReadAll(IBufferReader<byte> reader)
        {
            while (reader.RemainingBytes > 0)
            {
                PgpPacket packet = PgpPacketSerializer.ReadPacket(reader);

                yield return PgpPacketBodySerializer.Deserialize(packet);
            }
        }
    }
}
