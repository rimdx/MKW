using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class AdminSession : IDisposable
    {
        private readonly IDatabase db;
        private readonly IDatabaseAdmin admin;

        private readonly AsymmetricTransformer transformer;

        public AdminSession(IDatabase db /* reference */,
                            IDatabaseAdmin admin /* reference */,
                            ReadOnlySpan<byte> privateKey)
        {
            this.db = db;
            this.admin = admin;

            transformer = AsymmetricTransformer.Open(admin.PublicKey.Span, privateKey);
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

            foreach (Memory<byte> trust in admin.Trust)
            {
                if (transformer.Verify(user.PublicKey.Span, trust.Span))
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
            IDatabaseUser user = db.OpenUser(userId, DatabaseOpenMode.ReadOnly);

            Memory<byte> signature = transformer.Sign(user.PublicKey.Span);

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
        }

        public void Dispose()
        {
            transformer.Dispose();
        }
    }
}
