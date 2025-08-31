using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class UserTrustController : UserTrustProvider, ITrustProvider, ITrustController, IDisposable
    {
        protected readonly IAsymmetricPrivateTransformer privateKey;

        public UserTrustController(ClientSession client, UserSession user)
            : base(client, user.DatabaseUser)
        {
            privateKey = user.Transformer;
        }

        public void UpdateTrust(UserId userId, Trust trust)
        {
            IDatabaseUser user = client.OpenDatabaseUser(userId, true);

            ReadOnlyMemory<byte> signature = privateKey.Sign(user.PublicKey.Span);

            if (trust == Trust.ExplicitTrust)
            {
                me.AddTrust(signature);
            }
            else if (trust == Trust.None)
            {
                me.DeleteTrust(signature);
            }
            else
            {
                throw new ArgumentException("Invalid trust value.", nameof(trust));
            }

            me.Save();
        }

        public override void Dispose()
        {
            // no-op

            // In this case, the UserTrustProvider.key is actually managed by
            // caller, since the user is given by a reference.
        }
    }
}
