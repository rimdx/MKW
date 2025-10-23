// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;

namespace MKW.Storage
{
    public interface IDatabase3
    {
        public interface ISerializer
        {
            BlobSecretEntry SerializeEntry(DatabaseEntry entry);
            DatabaseEntry DeserializeEntry(BlobSecretEntry blob);

            BlobUser SerializeUser(DatabaseUser user);
            DatabaseUser DeserializeUser(BlobUser blob);

            ReadOnlyMemory<byte> SerializeProtectedData(DatabaseUserProtectedData obj);
        }

        public interface ISnapshot
            : ISerializer
            , IDisposable
        {
            DatabaseEntry OpenEntry();
            bool HasUser(UserId id);
            IEnumerable<DatabaseUser> EnumerateUsers();

            DatabaseEntry OpenEntry(EntryId id);
            bool HasEntry(EntryId id);
            IEnumerable<DatabaseEntry> EnumerateEntries();
        }

        public interface ITransaction
            : ISnapshot
            , ISerializer
            , IDisposable
        {
            void CreateUser(UserId id, DatabaseUser user);
            void UpdateUser(UserId id, DatabaseUser user);
            bool DeleteUser(UserId id);

            void CreateEntry(EntryId id, DatabaseEntry entry);
            void UpdateEntry(EntryId id, DatabaseEntry entry);
            bool DeleteEntry(EntryId id);

            void Commit();
        }

        ITransaction BeginTransaction();

        ISnapshot CreateSnapshot();

        DatabaseConfiguration GetConfiguration();

        Task<bool> WaitForDatabaseChangesAsync(CancellationToken cancellationToken);
        void ReloadDatabaseFile();
    }
}
