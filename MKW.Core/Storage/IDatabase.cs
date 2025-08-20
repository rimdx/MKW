namespace MKW.Core.Storage
{
    public interface IDatabase : ISavable
    {
        IDatabaseUser OpenUser(Guid id, DatabaseOpenMode mode, out bool created);
        IDatabaseUser OpenUser(Guid id, DatabaseOpenMode mode) => OpenUser(id, mode, out _);
        
        bool DeleteUser(Guid id);
        IEnumerable<IDatabaseUser> EnumerateUsers();

        IDatabaseAdmin OpenAdmin();

        IDatabaseEntry OpenEntry(Guid id, DatabaseOpenMode mode, out bool created);
        IDatabaseEntry OpenEntry(Guid id, DatabaseOpenMode mode) => OpenEntry(id, mode, out _);

        bool DeleteEntry(Guid id);
        IEnumerable<IDatabaseEntry> EnumerateEntries();
    }
}
