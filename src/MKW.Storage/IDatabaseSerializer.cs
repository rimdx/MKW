// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage
{
    public interface IDatabaseSerializer
    {
        BlobSecretEntry SerializeEntry(DatabaseEntry entry);
        DatabaseEntry DeserializeEntry(BlobSecretEntry blob);

        BlobUser SerializeUser(DatabaseUser user);
        DatabaseUser DeserializeUser(BlobUser blob);

        ReadOnlyMemory<byte> SerializeProtectedData(DatabaseUserProtectedData obj);
    }
}
