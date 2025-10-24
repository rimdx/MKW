// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage.JSON
{
    public partial class MemoryDatabaseSession
    {
        private abstract class SerializerBase
            : IDatabaseSerializer
        {
            public DatabaseEntry DeserializeEntry(BlobSecretEntry blob)
            {
                throw new NotImplementedException();
            }

            public DatabaseUser DeserializeUser(BlobUser blob)
            {
                throw new NotImplementedException();
            }

            public BlobSecretEntry SerializeEntry(DatabaseEntry entry)
            {
                throw new NotImplementedException();
            }

            public ReadOnlyMemory<byte> SerializeProtectedData(DatabaseUserProtectedData obj)
            {
                throw new NotImplementedException();
            }

            public BlobUser SerializeUser(DatabaseUser user)
            {
                throw new NotImplementedException();
            }
        }
    }
}
