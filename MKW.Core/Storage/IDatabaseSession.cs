namespace MKW.Core.Storage
{
    public interface IDatabaseSession : IDisposable
    {
        void AddUser(Guid id, DatabaseUser user);
        DatabaseUser GetUser(Guid id);
        IEnumerable<DatabaseUser> EnumerateUsers();

        void UpdateEntry(Guid id, DatabaseSecretEntry? entry);
        DatabaseSecretEntry? QueryEntry(Guid id);
        IEnumerable<DatabaseSecretKeyedEntry> EnumerateEntries();

        void Save();
    }
}
