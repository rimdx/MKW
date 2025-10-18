// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;

namespace MKW.Core.Serialization.OpenPgp
{
    public sealed record class PgpArmouredMessage
    {
        public required string MessageTypeHeader { get; init; }

        public required IReadOnlyCollection<PgpArmourHeader> Headers { get; init; }

        public required ReadOnlyMemory<byte> Data { get; init; }

        public IBufferReader CreateReader()
        {
            return new ArrayBufferReader(Data);
        }
    }
}
