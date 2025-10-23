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
            void CreateUser(DatabaseUser user);
            void UpdateUser(DatabaseUser user);
            bool DeleteUser(UserId id);

            void CreateEntry(DatabaseEntry entry);
            void UpdateEntry(DatabaseEntry entry);
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
