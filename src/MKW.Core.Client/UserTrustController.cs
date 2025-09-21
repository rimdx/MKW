using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    public class UserTrustController
        : UserTrustProvider
        , ITrustProvider
        , ITrustController
        , IDisposable
    {
        protected readonly IAsymmetricPrivateTransformer privateKey;

        public UserTrustController(IDatabase database,
                                   ICryptographyProvider crypto,
                                   IDatabaseUser me,
                                   IAsymmetricPrivateTransformer privateKey)
            : base(database, crypto, me)
        {
            this.privateKey = privateKey;
        }

        public void AddTrust(UserId userId)
        {
            IDatabaseUser user = database.OpenUser(userId, true);
            ReadOnlyMemory<byte> signature = privateKey.Sign(user.PublicKey.Span);

            me.AddTrust(signature);
            me.Save();
        }

        public void RemoveTrust(UserId userId)
        {
            IDatabaseUser user = database.OpenUser(userId, true);
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
