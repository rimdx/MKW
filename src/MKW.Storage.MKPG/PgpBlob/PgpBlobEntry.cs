// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.MKPG.PgpBlob
{
    internal sealed record class PgpBlobEntry
    {
        public required BlobId Id { get; init; }
        public required ReadOnlyMemory<byte> Data { get; init; }
        public required string Type { get; init; }
    }
}
