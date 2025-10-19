// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;

namespace MKW.Core.Serialization.OpenPgp
{
    public sealed record class PgpArmouredMessage
    {
        public required string MessageTypeHeader { get; init; }

        public required IReadOnlyCollection<PgpArmourHeader> Headers { get; init; }

        public PgpArmourHeader GetHeader(string name)
        {
            foreach (PgpArmourHeader header in Headers)
            {
                if (header.Key == name)
                {
                    return header;
                }
            }

            throw new Exception($"Header {name} does not exist.");
        }

        public required ReadOnlyMemory<byte> Data { get; init; }

        public IBufferReader<byte> CreateReader()
        {
            return new ArrayBufferReader<byte>(Data);
        }
    }
}
