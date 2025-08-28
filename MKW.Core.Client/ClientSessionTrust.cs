using MKW.Core.Client.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public partial class ClientSession : IDisposable
    {
        public IEnumerable<UserInfo> EnumerateUsersTrust()
        {
            IDatabaseUser admin = Database.OpenAdmin(true);
            using UserTrustProvider trustProvider = new UserTrustProvider(this, admin);

            IEnumerable<UserInfo> trust = trustProvider.EnumerateImplicitlyTrustedUsers();

            // Convert IEnumerable to an array, before returning from function,
            // because outside the trustProvider will be disposed.
            //
            // Dear .NET, why??
            return trust.ToArray();
        }

        public IEnumerable<UserInfo> EnumerateUsersTrust(UserId userId)
        {
            IDatabaseUser user = OpenDatabaseUser(userId, true);
            using UserTrustProvider trustProvider = new UserTrustProvider(this, user);

            foreach (UserInfo trust in trustProvider.EnumerateImplicitlyTrustedUsers())
            {
                yield return trust;
            }
        }
    }
}
