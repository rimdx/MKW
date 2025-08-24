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
