using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class UserTrustProvider : IDisposable
    {
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
            IDatabaseUser admin = client.Database.OpenAdmin(true);

            foreach (IDatabaseUser user in client.EnumerateDatabaseUsers())
            {
                UserInfo notify = UserInfo.FromDatabaseUser(user);
                notify.Trust = VerifyTrust(user);
                yield return notify;
            }
        }

        public IEnumerable<UserInfo> EnumerateImplicitlyTrustedUsers()
        {
            UserInfo notify = UserInfo.FromDatabaseUser(me);
            notify.Trust = Trust.FullTrust;

            yield return notify;

            foreach (IDatabaseUser user in EnumerateExplicitlyTrustedUsers())
            {
                using UserTrustProvider child = new UserTrustProvider(client, user);

                foreach (UserInfo trust in child.EnumerateUsersTrust())
                {
                    trust.Trust = Trust.ImplicitTrust;
                    yield return trust;
                }
            }
        }

        public IEnumerable<IDatabaseUser> EnumerateExplicitlyTrustedUsers()
        {
            foreach (IDatabaseUser user in client.EnumerateDatabaseUsers())
            {
                if (VerifyTrust2(user) == Trust.FullTrust)
                {
                    yield return user;
                }
            }
        }

        public IEnumerable<UserInfo> EnumerateExplicitlyTrustedUsersInfo()
        {
            foreach (IDatabaseUser user in EnumerateExplicitlyTrustedUsers())
            {
                UserInfo notify = UserInfo.FromDatabaseUser(user);
                notify.Trust = Trust.FullTrust;
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

        private Trust VerifyTrust2(IDatabaseUser user)
        {
            foreach (ReadOnlyMemory<byte> trust in me.EnumerateTrust())
            {
                if (key.Verify(user.PublicKey.Span, trust.Span))
                {
                    return Trust.FullTrust;
                }
            }

            return Trust.None;
        }

        public virtual void Dispose()
        {
            key.Dispose();
        }
    }
}
