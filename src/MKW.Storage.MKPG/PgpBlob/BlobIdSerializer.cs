// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;

namespace MKW.Storage.MKPG.PgpBlob
{
    internal static class BlobIdSerializer
    {
        public static string Serialize(BlobId blobId)
        {
            return Base16Convert.GetString(blobId.GetBytes().Span);
        }

        public static BlobId Deserialize(string str)
        {
            return BlobId.From(Base16Convert.GetBytes(str));
        }
    }
}
