using MKW.Core;

namespace MKW.Storage.MKPG
{
    public sealed class MKPGDatabase : IDatabase, IDisposable
    {
        public MKPGDatabase()
        {
        }

        // Entry
        public void CreateEntry(EntryId id, DatabaseEntry entry)
        {
            throw new NotImplementedException();
        }

        public void UpdateEntry(EntryId id, DatabaseEntry entry)
        {
            throw new NotImplementedException();
        }

        public DatabaseEntry OpenEntry(EntryId id)
        {
            throw new NotImplementedException();
        }

        public bool DeleteEntry(EntryId id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DatabaseEntry> EnumerateEntries()
        {
            throw new NotImplementedException();
        }

        public bool HasEntry(EntryId id)
        {
            throw new NotImplementedException();
        }

        // User
        public void CreateUser(UserId id, DatabaseUser user)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(UserId id, DatabaseUser user)
        {
            throw new NotImplementedException();
        }

        public DatabaseUser OpenUser(UserId id)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(UserId id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DatabaseUser> EnumerateUsers()
        {
            throw new NotImplementedException();
        }

        public bool HasUser(UserId id)
        {
            throw new NotImplementedException();
        }

        // Misc

        public void ReloadDatabaseFile()
        {
            throw new NotImplementedException();
        }

        public Task<bool> WaitForDatabaseChangesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
