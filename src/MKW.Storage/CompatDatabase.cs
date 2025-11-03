// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;

namespace MKW.Storage
{
    public sealed class CompatDatabase : IDatabase
    {
        private readonly IDatabaseNG proxy;
        private readonly IDatabaseSerializer serializer;

        public CompatDatabase(IDatabaseNG proxy, IDatabaseSerializer serializer)
        {
            this.proxy = proxy;
            this.serializer = serializer;
        }

        // Entry
        public void CreateEntry(EntryId id, DatabaseEntry entry)
        {
            using (IDatabaseNG.ITransaction transaction = proxy.BeginTransaction())
            {
                transaction.CreateEntry(entry);
                transaction.Commit();
            }
        }

        public void UpdateEntry(EntryId id, DatabaseEntry entry)
        {
            using (IDatabaseNG.ITransaction transaction = proxy.BeginTransaction())
            {
                transaction.UpdateEntry(entry);
                transaction.Commit();
            }
        }

        public DatabaseEntry OpenEntry(EntryId id)
        {
            IDatabaseNG.ISnapshot snapshot = proxy.CreateSnapshot();

            return snapshot.OpenEntry(id);
        }

        public bool DeleteEntry(EntryId id)
        {
            bool result;
            using (IDatabaseNG.ITransaction transaction = proxy.BeginTransaction())
            {
                result = transaction.DeleteEntry(id);
                transaction.Commit();
            }

            return result;
        }

        public bool HasEntry(EntryId id)
        {
            IDatabaseNG.ISnapshot snapshot = proxy.CreateSnapshot();

            return snapshot.HasEntry(id);
        }

        public IEnumerable<DatabaseEntry> EnumerateEntries()
        {
            IDatabaseNG.ISnapshot snapshot = proxy.CreateSnapshot();

            foreach (DatabaseEntry entry in snapshot.EnumerateEntries())
            {
                yield return entry;
            }
        }

        // Users
        public DatabaseUser OpenUser(UserId id)
        {
            IDatabaseNG.ISnapshot snapshot = proxy.CreateSnapshot();

            return snapshot.OpenUser(id);
        }

        public void CreateUser(UserId id, DatabaseUser user)
        {
            using (IDatabaseNG.ITransaction transaction = proxy.BeginTransaction())
            {
                transaction.CreateUser(user);
                transaction.Commit();
            }
        }

        public void UpdateUser(UserId id, DatabaseUser user)
        {
            using (IDatabaseNG.ITransaction transaction = proxy.BeginTransaction())
            {
                transaction.UpdateUser(user);
                transaction.Commit();
            }
        }

        public bool DeleteUser(UserId id)
        {
            bool result;
            using (IDatabaseNG.ITransaction transaction = proxy.BeginTransaction())
            {
                result = transaction.DeleteUser(id);
                transaction.Commit();
            }

            return result;
        }

        public bool HasUser(UserId id)
        {
            IDatabaseNG.ISnapshot snapshot = proxy.CreateSnapshot();

            return snapshot.HasUser(id);
        }

        public IEnumerable<DatabaseUser> EnumerateUsers()
        {
            IDatabaseNG.ISnapshot snapshot = proxy.CreateSnapshot();

            foreach (DatabaseUser user in snapshot.EnumerateUsers())
            {
                yield return user;
            }
        }

        public DatabaseConfiguration GetConfiguration()
        {
            return proxy.GetConfiguration();
        }

        public Task<bool> WaitForDatabaseChangesAsync(CancellationToken cancellationToken)
        {
            return proxy.WaitForDatabaseChangesAsync(cancellationToken);
        }

        public void ReloadDatabaseFile()
        {
            proxy.ReloadDatabaseFile();
        }

        public ReadOnlyMemory<byte> SerializeProtectedData(DatabaseUserProtectedData obj)
        {
            return serializer.SerializeProtectedData(obj);
        }

        public void Dispose()
        {
            proxy.Dispose();
        }
    }
}
