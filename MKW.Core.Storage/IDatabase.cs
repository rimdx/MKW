namespace MKW.Core.Storage
{
    public interface IDatabase : ISavable, IDisposable
    {
        // User Management
        IDatabaseUser CreateUser(UserId id);
        IDatabaseUser OpenUser(UserId id, bool readOnly);
        bool DeleteUser(UserId id);

        // ReadOnly
        bool HasUser(UserId id);
        IEnumerable<IDatabaseUser> EnumerateUsers();

        IDatabaseAdmin CreateAdmin();
        IDatabaseAdmin OpenAdmin(bool readOnly);

        // Entry Management
        IDatabaseEntry CreateEntry(EntryId id);
        IDatabaseEntry OpenEntry(EntryId id, bool readOnly);
        bool DeleteEntry(EntryId id);

        // ReadOnly
        bool HasEntry(EntryId id);
        IEnumerable<IDatabaseEntry> EnumerateEntries();
    }
}
