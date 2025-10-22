// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage
{
    public interface IDatabase2 : IDatabase, IDisposable
    {
        // User Management
        Blob OpenUser2(BlobId id);
        void CreateUser2(Blob user);
        void UpdateUser2(Blob user);
        IEnumerable<Blob> EnumerateUsers2();

        Blob SerializeUser2(DatabaseUser user);
        DatabaseUser DeserializeUser2(Blob blob);

        // Entry Management
        Blob OpenEntry2(BlobId id);
        void CreateEntry2(Blob entry);
        void UpdateEntry2(Blob entry);
        IEnumerable<Blob> EnumerateEntries2();

        Blob SerializeEntry2(DatabaseEntry entry);
        DatabaseEntry DeserializeEntry2(Blob blob);
    }
}
