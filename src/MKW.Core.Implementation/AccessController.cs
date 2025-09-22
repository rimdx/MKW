using MKW.Core.Storage;

namespace MKW.Core.Implementation
{
    public class AccessController : IDisposable
    {
        private readonly HashSet<UserId> access;

        public AccessController(IEnumerable<UserId> access)
        {
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

            return new AccessController(access);
        }

        public static AccessController Open(IDatabase database,
                                            EntryId entryId)
        {
            IDatabaseEntry entry = database.OpenEntry(entryId, true);
            return new AccessController(entry.Keys.Keys);
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
