using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class UserTrustProvider : IDisposable
    {
        public UserId UserId => me.Id;

        protected readonly ClientSession client;
        protected readonly IDatabaseUser me;
        protected readonly AsymmetricTransformer key;

        public UserTrustProvider(ClientSession client, IDatabaseUser user)
            : this(client, user, AsymmetricTransformer.Open(user.PublicKey.Span))
        {
        }

        public UserTrustProvider(ClientSession client, IDatabaseUser user,
                                 AsymmetricTransformer key)
        {
            this.client = client;
            me = user;
            this.key = key;
        }

        public IEnumerable<UserInfo> EnumerateUsersTrust()
        {
            return EnumerateImplicitlyTrustedUsers();
        }

        public IEnumerable<UserInfo> EnumerateImplicitlyTrustedUsers()
        {
            using UserTrustWorker worker = new UserTrustWorker(client, this, client.EnumerateDatabaseUsers());

            while (worker.Iterate())
                continue;

            return worker.EnumerateTrustedUsers();
        }

        public IEnumerable<IDatabaseUser> EnumerateExplicitlyTrustedUsers()
        {
            foreach (IDatabaseUser user in client.EnumerateDatabaseUsers())
            {
                if (VerifyTrust(user) == Trust.ExplicitTrust)
                {
                    yield return user;
                }
            }
        }

        public IEnumerable<UserInfo> EnumerateExplicitlyTrustedUsersInfo()
        {
            foreach (IDatabaseUser user in EnumerateExplicitlyTrustedUsers())
            {
                yield return UserInfo.FromDatabaseUser(user, Trust.ExplicitTrust);
            }
        }

        internal Trust VerifyTrust(IDatabaseUser user)
        {
            foreach (ReadOnlyMemory<byte> trust in me.EnumerateTrust())
            {
                if (key.Verify(user.PublicKey.Span, trust.Span))
                {
                    return Trust.ExplicitTrust;
                }
            }

            return Trust.None;
        }

        public UserTrustProvider CreateChildTrustProvider(IDatabaseUser user)
        {
            return new UserTrustProvider(client, user);
        }

        public virtual void Dispose()
        {
            key.Dispose();
        }
    }
}
