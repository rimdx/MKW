namespace MKW.Core.Storage
{
    public interface IDatabase : ISavable
    {
        // User Management
        IDatabaseUser CreateUser(Guid id);
        IDatabaseUser OpenUser(Guid id, bool readOnly);
        bool DeleteUser(Guid id);

        // ReadOnly
        bool HasUser(Guid id);
        IEnumerable<IDatabaseUser> EnumerateUsers();

        IDatabaseAdmin OpenAdmin(out bool created);
        IDatabaseAdmin OpenAdmin() => OpenAdmin(out _);

        // Entry Management
        IDatabaseEntry CreateEntry(Guid id);
        IDatabaseEntry OpenEntry(Guid id, bool readOnly);
        bool DeleteEntry(Guid id);

        // ReadOnly
        bool HasEntry(Guid id);
        IEnumerable<IDatabaseEntry> EnumerateEntries();
    }
}
