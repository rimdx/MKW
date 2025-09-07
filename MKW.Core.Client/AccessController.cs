using MKW.Core.Client.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class AccessController : IEntryAccessController, IDisposable
    {
        private readonly ClientSession client;
        private readonly ITrustProvider trustProvider;
        private readonly IDatabaseEntry entry;
        private readonly HashSet<UserId> access;

        public AccessController(ClientSession client,
                                ITrustProvider trustProvider,
                                IDatabaseEntry entry)
        {
            this.client = client;
            this.trustProvider = trustProvider;
            this.entry = entry;

            // TODO: access = [.. entry.Keys.Keys];
            access = [];
            foreach (UserInfo user in trustProvider.EnumerateImplicitlyTrustedUsers())
            {
                access.Add(user.Id);
            }
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
