// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage
{
    public sealed partial class MDatabase
    {
        private abstract class SerializerBase
            : IDatabaseSerializer
        {
            protected readonly IDatabaseSerializer serializer;

            protected SerializerBase(IDatabaseSerializer serializer)
            {
                this.serializer = serializer;
            }

            // Entry
            public DatabaseEntry DeserializeEntry(BlobSecretEntry blob)
            {
                return serializer.DeserializeEntry(blob);
            }

            public BlobSecretEntry SerializeEntry(DatabaseEntry entry)
            {
                return serializer.SerializeEntry(entry);
            }

            // User
            public DatabaseUser DeserializeUser(BlobUser blob)
            {
                return serializer.DeserializeUser(blob);
            }

            public BlobUser SerializeUser(DatabaseUser user)
            {
                return serializer.SerializeUser(user);
            }

            // Payloads
            public ReadOnlyMemory<byte> SerializeProtectedData(DatabaseUserProtectedData obj)
            {
                return serializer.SerializeProtectedData(obj);
            }
        }
    }
}
