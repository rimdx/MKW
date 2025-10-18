// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;

namespace MKW.Storage.MKPG
{
    internal sealed record class BlobEntry(
        BlobId Id,
        ReadOnlyMemory<byte> Data)
    {
        public IBufferReader<byte> CreateReader()
        {
            return new ArrayBufferReader<byte>(Data);
        }
    }
}
