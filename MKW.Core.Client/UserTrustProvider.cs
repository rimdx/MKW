using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class UserTrustProvider : ITrustProvider, IDisposable
    {
        public UserId UserId => me.Id;

        protected readonly ClientSession client;
        protected readonly IDatabaseUser me;
        protected readonly IAsymmetricPublicTransformer publicKey;

        public UserTrustProvider(ClientSession client, IDatabaseUser user)
            : this(client, user, AsymmetricTransformer.Open(user.PublicKey.Span))
        {
        }

        public UserTrustProvider(ClientSession client, IDatabaseUser user,
                                 IAsymmetricPublicTransformer publicKey)
        {
            this.client = client;
            me = user;
            this.publicKey = publicKey;
        }

        public IEnumerable<UserInfo> EnumerateImplicitlyTrustedUsers()
        {
            using UserTrustWorker worker = new UserTrustWorker(client, this, client.EnumerateDatabaseUsers());

            while (worker.Iterate())
                continue;

            return worker.EnumerateTrustedUsers();
        }

        public IEnumerable<UserInfo> EnumerateExplicitlyTrustedUsers()
        {
            foreach (IDatabaseUser user in client.EnumerateDatabaseUsers())
            {
                if (GetExplicitTrust(user.PublicKey.Span) == Trust.ExplicitTrust)
                {
                    yield return UserInfo.FromDatabaseUser(user, Trust.ExplicitTrust);
                }
            }
        }

        public Trust GetExplicitTrust(ReadOnlySpan<byte> publicKey)
        {
            foreach (ReadOnlyMemory<byte> trust in me.EnumerateTrust())
            {
                if (this.publicKey.Verify(publicKey, trust.Span))
                {
                    return Trust.ExplicitTrust;
                }
            }

            return Trust.None;
        }

        public virtual void Dispose()
        {
            publicKey.Dispose();
        }
    }
}
