// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage
{
    public sealed record class BlobUser : Blob
    {
        public override T Visit<T>(IVisitor<T> visitor)
        {
            return visitor.VisitUser(this);
        }
    }
}
