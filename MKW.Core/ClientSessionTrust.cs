using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public partial class ClientSession : IDisposable
    {
        public IEnumerable<UserInfo> EnumerateUsersTrust()
        {
            IDatabaseAdmin admin = Database.OpenAdmin(out bool created);

            if (created)
            {
                foreach (IDatabaseUser user in Database.EnumerateUsers())
                {
                    UserInfo notify = UserInfo.FromDatabaseUser(user);
                    // Implicitly trust all users if no admin exists
                    notify.Trust = Trust.FullTrust;
                    yield return notify;
                }
            }
            else
            {
                using AsymmetricTransformer adminKey = AsymmetricTransformer.Open(admin.PublicKey.Span);

                foreach (IDatabaseUser user in Database.EnumerateUsers())
                {
                    UserInfo notify = UserInfo.FromDatabaseUser(user);
                    notify.Trust = VerifyTrust(user, admin, adminKey);
                    yield return notify;
                }
            }
        }

        private Trust VerifyTrust(IDatabaseUser user, IDatabaseAdmin admin, AsymmetricTransformer adminKey)
        {
            foreach (ReadOnlyMemory<byte> trust in admin.EnumerateTrust())
            {
                if (adminKey.Verify(user.PublicKey.Span, trust.Span))
                {
                    return Trust.FullTrust;
                }
            }

            return Trust.None;
        }
    }
}
