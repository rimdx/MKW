// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Storage.MKPG.PgpBlob;

namespace MKW.Storage.MKPG.BlobStore
{
    internal interface IBlobStorage
    {
        void Create(PgpBlobEntry entry);

        void Update(PgpBlobEntry entry);

        PgpBlobEntry Open(BlobId id);

        bool Delete(BlobId id);

        IEnumerable<PgpBlobEntry> Enumerate();

        bool Exists(BlobId id);
    }
}
