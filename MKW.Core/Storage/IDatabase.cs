namespace MKW.Core.Storage
{
    public interface IDatabase : IDisposable
    {
        void AddUser(Guid id, DatabaseUser user);
        DatabaseUser GetUser(Guid id);
        IEnumerable<DatabaseUser> EnumerateUsers();

        void UpdateAdmin(AdminUser user);
        AdminUser GetAdmin();

        void UpdateEntry(Guid id, DatabaseSecretEntry? entry);
        DatabaseSecretEntry? QueryEntry(Guid id);
        IEnumerable<DatabaseSecretKeyedEntry> EnumerateEntries();

        void Save();
    }
}
