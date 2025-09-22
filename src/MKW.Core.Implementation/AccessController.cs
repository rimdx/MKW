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
                                              ITrustProvider trustProvider,
                                              IDatabaseEntry entry)
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

        public IEnumerable<UserInfo> EnumerateAccess()
        {
            foreach (UserId user in access)
            {
                IDatabaseUser databaseUser = database.OpenUser(user, true);
                yield return UserInfo.FromDatabaseUser(databaseUser);
            }
        }

        public void Dispose()
        {
        }
    }
}
