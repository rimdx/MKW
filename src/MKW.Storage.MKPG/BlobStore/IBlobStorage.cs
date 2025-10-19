// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.MKPG.BlobStore
{
    internal interface IBlobStorage
    {
        void Create(BlobEntry entry);

        void Update(BlobEntry entry);

        BlobEntry Open(BlobId id);

        bool Delete(BlobId id);

        IEnumerable<BlobEntry> Enumerate();

        bool Exists(BlobId id);
    }
}
