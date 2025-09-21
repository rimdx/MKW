using MKW.Core.Notify;
using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    public class UserTrustProvider : ITrustProvider, IDisposable
    {
        public UserId UserId => me.Id;

        protected readonly IDatabase database;
        protected readonly ICryptographyProvider crypto;
        protected readonly IDatabaseUser me;
        protected readonly ITrustVerifier trustVerifier;

        public UserTrustProvider(IDatabase database,
                                 ICryptographyProvider crypto,
                                 IDatabaseUser me)
        {
            this.database = database;
            this.crypto = crypto;
            this.me = me;
            trustVerifier = new UserTrustVerifier(crypto, me);
        }

        private IEnumerable<ITrustWorkerNode> EnumerateWorkerNodes()
        {
            foreach (IDatabaseUser user in database.EnumerateUsers())
            {
                yield return new UserTrustProviderWorkerNode(crypto, user);
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
            foreach (IDatabaseUser user in database.EnumerateUsers())
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
