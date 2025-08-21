using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public partial class ClientSession : IDisposable
    {
        public IEnumerable<UserInfo> EnumerateUsersTrust()
        {
            IDatabaseAdmin admin = Database.OpenAdmin();

            using AsymmetricTransformer adminKey = AsymmetricTransformer.Open(admin.PublicKey.Span);

            foreach (IDatabaseUser user in Database.EnumerateUsers())
            {
                yield return GetTrust(user, admin, adminKey);
            }
        }

        private UserInfo GetTrust(IDatabaseUser user, IDatabaseAdmin admin, AsymmetricTransformer adminKey)
        {
            UserInfo notify = UserInfo.FromDatabaseUser(user);

            foreach (ReadOnlyMemory<byte> trust in admin.EnumerateTrust())
            {
                if (adminKey.Verify(user.PublicKey.Span, trust.Span))
                {
                    notify.Trust = Trust.FullTrust;
                    return notify;
                }
            }

            notify.Trust = Trust.None;
            return notify;
        }
    }
}
