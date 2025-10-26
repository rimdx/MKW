// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Storage.MKPG.PgpBlob;

namespace MKW.Storage.MKPG.BlobStore
{
    internal static class PgpBlobEntryExtensions
    {
        public static Blob GetTypedBlob(this PgpBlobEntry blob)
        {
            if (blob.Type == MKPGConstants.ArmourTypeHeaders.User)
            {
                return new BlobUser
                {
                    Id = blob.Id,
                    Data = blob.Data,
                };
            }
            else if (blob.Type == MKPGConstants.ArmourTypeHeaders.Entry)
            {
                return new BlobSecretEntry
                {
                    Id = blob.Id,
                    Data = blob.Data,
                };
            }
            else
            {
                throw new Exception($"Unknown blob type: {blob.Type}");
            }
        }
    }
}
