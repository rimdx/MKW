namespace MKW.Core.Storage
{
    public interface IDatabase : IDisposable
    {
        // User Management
        DatabaseUser OpenUser(UserId id);

        void CreateUser(UserId id, DatabaseUser user);
        void UpdateUser(UserId id, DatabaseUser user);
        bool DeleteUser(UserId id);

        bool HasUser(UserId id);
        IEnumerable<DatabaseUser> EnumerateUsers();

        // Entry Management
        DatabaseEntry OpenEntry(EntryId id);

        void CreateEntry(EntryId id, DatabaseEntry entry);
        void UpdateEntry(EntryId id, DatabaseEntry entry);
        bool DeleteEntry(EntryId id);

        bool HasEntry(EntryId id);
        IEnumerable<DatabaseEntry> EnumerateEntries();

        Task<bool> WaitForDatabaseChangesAsync(CancellationToken cancellationToken);
        void ReloadDatabaseFile();
    }
}
