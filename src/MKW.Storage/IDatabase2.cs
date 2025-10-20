// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage
{
    public interface IDatabase2 : IDatabase, IDisposable
    {
        // User Management
        BlobEntry OpenUser2(BlobId id);
        void CreateUser2(BlobEntry user);
        void UpdateUser2(BlobEntry user);
        IEnumerable<BlobEntry> EnumerateUsers2();

        BlobEntry SerializeUser2(DatabaseUser user);
        DatabaseUser DeserializeUser2(BlobEntry blob);

        // Entry Management
        BlobEntry OpenEntry2(BlobId id);
        void CreateEntry2(BlobEntry entry);
        void UpdateEntry2(BlobEntry entry);
        IEnumerable<BlobEntry> EnumerateEntries2();

        BlobEntry SerializeEntry2(DatabaseEntry entry);
        DatabaseEntry DeserializeEntry2(BlobEntry blob);
    }
}
