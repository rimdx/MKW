namespace MKW.Core.Storage
{
    public interface IDatabase : ISavable, IDisposable
    {
        // User Management
        IDatabaseUser OpenUser(UserId id);

        void CreateUser(UserId id, IDatabaseUser user);
        void UpdateUser(UserId id, IDatabaseUser user);
        bool DeleteUser(UserId id);

        bool HasUser(UserId id);
        IEnumerable<IDatabaseUser> EnumerateUsers();

        // Entry Management
        DatabaseEntry OpenEntry(EntryId id);

        void CreateEntry(EntryId id, DatabaseEntry entry);
        void UpdateEntry(EntryId id, DatabaseEntry entry);
        bool DeleteEntry(EntryId id);

        bool HasEntry(EntryId id);
        IEnumerable<DatabaseEntry> EnumerateEntries();
    }
}
