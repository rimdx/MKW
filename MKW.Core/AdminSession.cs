using MKW.Core.Client.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class AdminSession : UserSession, IDisposable
    {
        private readonly IDatabaseAdmin admin;

        public AdminSession(ClientSession client /* reference */,
                            IDatabaseAdmin admin /* reference */,
                            ReadOnlySpan<byte> privateKey)
            : base(client, admin, privateKey)
        {
            this.admin = admin;
        }

        public void UpdateTrust(UserId userId, Trust trust)
        {
            IDatabaseUser user = client.Database.OpenUser(userId, true);

            ReadOnlyMemory<byte> signature = Transformer.Sign(user.PublicKey.Span);

            if (trust == Trust.FullTrust)
            {
                admin.AddTrust(signature);
            }
            else if (trust == Trust.None)
            {
                admin.DeleteTrust(signature);
            }
            else
            {
                throw new ArgumentException("Invalid trust value.", nameof(trust));
            }

            admin.Save();
        }
    }
}
