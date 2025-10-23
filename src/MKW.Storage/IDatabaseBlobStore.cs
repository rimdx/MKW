// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage
{
    public interface IDatabaseBlobStore : IDisposable
    {
        public interface ISnapshot
            : IDisposable
        {
            IEnumerable<Blob> Enumerate();
        }

        public interface ITransaction
            : ISnapshot
            , IDisposable
        {
            void Create(Blob blob);
            void Update(Blob blob);
            bool Delete(BlobId blobId);

            void Commit();
        }

        ITransaction BeginTransaction();
        ISnapshot CreateSnapshot();
    }
}
