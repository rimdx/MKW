// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.MKPG.BlobStore
{
    internal sealed record class PgpBlobEntry : Blob
    {
        public required string Type { get; init; }

        public override T Visit<T>(IVisitor<T> visitor)
        {
            throw new NotImplementedException();
        }
    }
}
