// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.MKPG.BlobStore
{
    internal static class BlobExtensions
    {
        public static PgpBlobEntry GetPgpBlob(this Blob blob)
        {
            return new PgpBlobEntry
            {
                Id = blob.Id,
                Data = blob.Data,
                Type = blob.Visit(new GetBlobTypeVisitor()),
            };
        }
    }
}
