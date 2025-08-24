using MKW.Core.Client.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class AdminSession : UserSession, IDisposable
    {
        public AdminSession(ClientSession client /* reference */,
                            IDatabaseUser admin /* reference */,
                            ReadOnlySpan<byte> privateKey)
            : base(client, admin, privateKey)
        {
        }

        public void UpdateTrust(UserId userId, Trust trust)
        {
            using UserTrustController trustController = new UserTrustController(client, this);

            trustController.UpdateTrust(userId, trust);
        }
    }
}
