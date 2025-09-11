using MKW.Core.Client.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class AccessController : IEntryAccessController, IDisposable
    {
        private readonly ClientSession client;
        private readonly HashSet<UserId> access;

        public AccessController(ClientSession client,
                                IEnumerable<UserId> access)
        {
            this.client = client;
            this.access = [.. access];
        }

        public static AccessController Create(ClientSession client,
                                              ITrustProvider trustProvider,
                                              IDatabaseEntry entry)
        {
            HashSet<UserId> access = [];
            foreach (UserInfo user in trustProvider.EnumerateImplicitlyTrustedUsers())
            {
                access.Add(user.Id);
            }

            return new AccessController(client, access);
        }

        public static AccessController Open(ClientSession client,
                                            IDatabaseEntry entry)
        {
            return new AccessController(client, entry.Keys.Keys);
        }

        public void AddAccess(UserId userId)
        {
            access.Add(userId);
        }

        public IEnumerable<UserInfo> EnumerateAccess()
        {
            foreach (UserId user in access)
            {
                IDatabaseUser databaseUser = client.OpenDatabaseUser(user, true);
                yield return UserInfo.FromDatabaseUser(databaseUser);
            }
        }

        public void Dispose()
        {
        }
    }
}
