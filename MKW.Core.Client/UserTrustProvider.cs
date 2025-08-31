using MKW.Core.Client.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class UserTrustProvider : ITrustProvider, IDisposable
    {
        public UserId UserId => me.Id;

        protected readonly ClientSession client;
        protected readonly IDatabaseUser me;
        protected readonly ITrustVerifier trustVerifier;

        public UserTrustProvider(ClientSession client, IDatabaseUser me)
        {
            this.client = client;
            this.me = me;
            trustVerifier = new UserTrustVerifier(client, me);
        }

        private IEnumerable<ITrustWorkerNode> EnumerateWorkerNodes()
        {
            foreach (IDatabaseUser user in client.EnumerateDatabaseUsers())
            {
                yield return new UserTrustProviderWorkerNode(client, user);
            }
        }

        public IEnumerable<UserInfo> EnumerateImplicitlyTrustedUsers()
        {
            using UserTrustWorker worker = new UserTrustWorker(UserId, EnumerateWorkerNodes());

            while (worker.Iterate())
                continue;

            return worker.EnumerateTrustedUsers();
        }

        public IEnumerable<UserInfo> EnumerateExplicitlyTrustedUsers()
        {
            foreach (IDatabaseUser user in client.EnumerateDatabaseUsers())
            {
                if (trustVerifier.GetTrust(user.PublicKey.Span) == Trust.ExplicitTrust)
                {
                    yield return UserInfo.FromDatabaseUser(user, Trust.ExplicitTrust);
                }
            }
        }

        public Trust GetExplicitTrust(ReadOnlySpan<byte> publicKey)
        {
            return trustVerifier.GetTrust(publicKey);
        }

        public Trust GetImplicitTrust(UserId userId)
        {
            foreach (UserInfo userTrust in EnumerateImplicitlyTrustedUsers())
            {
                if (userTrust.Id == userId)
                {
                    return userTrust.Trust;
                }
            }

            return Trust.None;
        }

        public virtual void Dispose()
        {
            trustVerifier.Dispose();
        }
    }
}
