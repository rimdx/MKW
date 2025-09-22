using MKW.Core.Storage;

namespace MKW.Core.Implementation
{
    public class AccessController : IDisposable
    {
        private readonly IDatabase database;
        private readonly HashSet<UserId> access;

        public AccessController(IDatabase database,
                                IEnumerable<UserId> access)
        {
            this.database = database;
            this.access = [.. access];
        }

        public static AccessController Create(IDatabase database,
                                              ITrustProvider trustProvider)
        {
            HashSet<UserId> access = [];
            foreach (UserInfo user in trustProvider.EnumerateTrustedUsers())
            {
                access.Add(user.Id);
            }

            return new AccessController(database, access);
        }

        public static AccessController Open(IDatabase database,
                                            IDatabaseEntry entry)
        {
            return new AccessController(database, entry.Keys.Keys);
        }

        public void AddAccess(UserId userId)
        {
            access.Add(userId);
        }

        public IEnumerable<UserId> EnumerateAccess()
        {
            foreach (UserId id in access)
            {
                yield return id;
            }
        }

        public void Dispose()
        {
        }
    }
}
