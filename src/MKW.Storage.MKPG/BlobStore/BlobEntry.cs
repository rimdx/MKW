// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;

namespace MKW.Storage.MKPG.BlobStore
{
    internal sealed record class BlobEntry
    {
        public required BlobId Id { get; init; }
        public required string Type { get; init; }
        public required ReadOnlyMemory<byte> Data { get; init; }

        public IBufferReader<byte> CreateReader()
        {
            return new ArrayBufferReader<byte>(Data);
        }
    }
}
