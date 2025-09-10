using MKW.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class UserTrustController
        : UserTrustProvider
        , ITrustProvider
        , ITrustController
        , IDisposable
    {
        protected readonly IAsymmetricPrivateTransformer privateKey;

        public UserTrustController(ClientSession client,
                                   ICryptographyProvider crypto,
                                   UserSession user)
            : base(client, crypto, user.DatabaseUser)
        {
            privateKey = user.Transformer;
        }

        public void AddTrust(UserId userId)
        {
            IDatabaseUser user = client.OpenDatabaseUser(userId, true);
            ReadOnlyMemory<byte> signature = privateKey.Sign(user.PublicKey.Span);

            me.AddTrust(signature);
            me.Save();
        }

        public void RemoveTrust(UserId userId)
        {
            IDatabaseUser user = client.OpenDatabaseUser(userId, true);
            ReadOnlyMemory<byte> signature = privateKey.Sign(user.PublicKey.Span);

            me.DeleteTrust(signature);
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
