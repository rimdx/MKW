using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class UserTrustProvider : IDisposable
    {
        private readonly ClientSession client;
        private readonly IDatabaseUser me;
        private readonly AsymmetricTransformer key;

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
            IDatabaseUser admin = client.Database.OpenAdmin(true);

            foreach (IDatabaseUser user in client.EnumerateDatabaseUsers())
            {
                UserInfo notify = UserInfo.FromDatabaseUser(user);
                notify.Trust = VerifyTrust(user);
                yield return notify;
            }
        }

        private Trust VerifyTrust(IDatabaseUser user)
        {
            // TODO: this is insecure!
            if (user.Id.IsAdmin)
            {
                // We always trust admins.
                // TODO: sign admins to handle potential fake admins
                return Trust.FullTrust;
            }

            foreach (ReadOnlyMemory<byte> trust in me.EnumerateTrust())
            {
                if (key.Verify(user.PublicKey.Span, trust.Span))
                {
                    return Trust.FullTrust;
                }
            }

            return Trust.None;
        }

        public void Dispose()
        {
            key.Dispose();
        }
    }
}
