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
        IDatabaseEntry OpenEntry(EntryId id);

        void CreateEntry(EntryId id, IDatabaseEntry entry);
        void UpdateEntry(EntryId id, IDatabaseEntry entry);
        bool DeleteEntry(EntryId id);

        bool HasEntry(EntryId id);
        IEnumerable<IDatabaseEntry> EnumerateEntries();
    }
}
