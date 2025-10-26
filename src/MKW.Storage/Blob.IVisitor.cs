// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage
{
    public abstract partial record class Blob
    {
        public interface IVisitor<T>
        {
            T VisitUser(BlobUser user);
            T VisitSecretEntry(BlobSecretEntry secretEntry);
        }
    }
}
