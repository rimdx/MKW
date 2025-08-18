namespace MKW.Core.Storage
{
    public interface IDatabaseSession : IDisposable
    {
        void AddUser(Guid id, User user);
        User GetUser(Guid id);

        void Save();
    }
}
