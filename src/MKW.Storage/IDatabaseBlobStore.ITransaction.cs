// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage
{
    public partial interface IDatabaseBlobStore
    {
        public interface ITransaction
            : ISnapshot
            , IDisposable
        {
            void Create(Blob blob);
            void Update(Blob blob);
            bool Delete(BlobId blobId);

            void Commit();
        }
    }
}
