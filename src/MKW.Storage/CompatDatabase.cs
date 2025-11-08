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

        // Users
        public DatabaseUser OpenUser(UserId id)
        {
            IDatabaseNG.ISnapshot snapshot = proxy.CreateSnapshot();

            return snapshot.OpenUser(id);
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

        public IDatabaseNG.ITransaction BeginTransaction()
        {
            return proxy.BeginTransaction();
        }

        public IDatabaseNG.ISnapshot CreateSnapshot()
        {
            return proxy.CreateSnapshot();
        }
    }
}
