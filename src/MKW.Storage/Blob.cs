// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;

namespace MKW.Storage
{
    public abstract partial record class Blob
    {
        public required BlobId Id { get; init; }
        public required ReadOnlyMemory<byte> Data { get; init; }

        public abstract T Visit<T>(IVisitor<T> visitor);

        public IBufferReader<byte> CreateReader()
        {
            return new ArrayBufferReader<byte>(Data);
        }
    }
}
