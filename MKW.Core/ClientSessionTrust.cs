using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public partial class ClientSession : IDisposable
    {
        public IEnumerable<UserInfo> EnumerateUsersTrust()
        {
            IDatabaseUser admin = Database.OpenAdmin(true);

            using AsymmetricTransformer adminKey = AsymmetricTransformer.Open(admin.PublicKey.Span);

            foreach (IDatabaseUser user in EnumerateDatabaseUsers())
            {
                UserInfo notify = UserInfo.FromDatabaseUser(user);
                notify.Trust = VerifyTrust(user, admin, adminKey);
                yield return notify;
            }
        }

        private Trust VerifyTrust(IDatabaseUser user, IDatabaseUser admin, AsymmetricTransformer adminKey)
        {
            // TODO: this is insecure!
            if (user.Id.IsAdmin)
            {
                // We always trust admins.
                // TODO: sign admins to handle potential fake admins
                return Trust.FullTrust;
            }

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
