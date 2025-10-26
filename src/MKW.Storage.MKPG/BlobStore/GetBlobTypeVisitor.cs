// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.MKPG.BlobStore
{
    internal sealed class GetBlobTypeVisitor : Blob.IVisitor<string>
    {
        public string VisitSecretEntry(BlobSecretEntry secretEntry)
        {
            return MKPGConstants.ArmourTypeHeaders.Entry;
        }

        public string VisitUser(BlobUser user)
        {
            return MKPGConstants.ArmourTypeHeaders.User;
        }
    }
}
