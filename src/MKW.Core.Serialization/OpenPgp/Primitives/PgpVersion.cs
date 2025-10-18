// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;
using System.Buffers;

namespace MKW.Core.Serialization.OpenPgp.Primitives
{
    internal sealed class PgpVersion
    {
        private readonly int expectedVersion;

        public PgpVersion(int expectedVersion)
        {
            this.expectedVersion = expectedVersion;
        }

        public void ConsumeVersion(IBufferReader<byte> reader)
        {
            int version = reader.ReadByte();

            if (version != expectedVersion)
            {
                throw new PgpVersionMismatchException(expectedVersion, version);
            }
            else
            {
                /* we're fine */
            }
        }

        public void Serialize(IBufferWriter<byte> writer)
        {
            writer.Write((byte)expectedVersion);
        }
    }
}
