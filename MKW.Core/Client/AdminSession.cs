using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class AdminSession : IDisposable
    {
        private readonly IDatabase db;
        private readonly AdminUser admin;

        private readonly AsymmetricTransformer transformer;

        public AdminSession(IDatabase db /* reference */,
                            AdminUser admin /* reference */,
                            byte[] privateKey)
        {
            this.db = db;
            this.admin = admin;

            transformer = AsymmetricTransformer.Open(admin.PublicKey, privateKey);
        }

        public IEnumerable<UserInfo> EnumerateUsersTrust()
        {
            foreach (DatabaseUser user in db.EnumerateUsers())
            {
                yield return GetTrust(user);
            }
        }

        private UserInfo GetTrust(DatabaseUser user)
        {
            UserInfo notify = UserInfo.FromDatabaseUser(user);

            foreach (byte[] trust in admin.Trust)
            {
                if (transformer.Verify(user.PublicKey, trust))
                {
                    notify.Trust = Trust.FullTrust;
                    return notify;
                }
            }

            notify.Trust = Trust.None;
            return notify;
        }

        public void UpdateTrust(Guid userId, Trust trust)
        {
            DatabaseUser user = db.GetUser(userId);

            byte[] signature = transformer.Sign(user.PublicKey);

            if (trust == Trust.FullTrust)
            {
                admin.Trust.Add(signature);
            }
            else if (trust == Trust.None)
            {
                admin.Trust.Remove(signature);
            }
            else
            {
                throw new ArgumentException("Invalid trust value.", nameof(trust));
            }

            db.UpdateAdmin(admin);
        }

        public void Dispose()
        {
            transformer.Dispose();
        }
    }
}
