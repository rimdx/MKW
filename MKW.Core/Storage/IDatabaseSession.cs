namespace MKW.Core.Storage
{
    public interface IDatabaseSession : IDisposable
    {
        void AddUser(Guid id, User user);
        User GetUser(Guid id);
        IEnumerable<User> EnumerateUsers();

        void UpdateEntry(Guid id, Entry? entry);
        Entry QueryEntry(Guid id);

        void Save();
    }
}
