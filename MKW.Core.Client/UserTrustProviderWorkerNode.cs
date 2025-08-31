using MKW.Core.Client.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    internal class UserTrustProviderWorkerNode : ITrustWorkerNode, IDisposable
    {
        private readonly ClientSession client;
        private readonly IDatabaseUser user;
        private ITrustVerifier? trustVerifier;

        public UserId Id => user.Id;

        public UserTrustProviderWorkerNode(ClientSession client, IDatabaseUser user)
        {
            this.client = client;
            this.user = user;
        }

        public Trust GetTrust(ITrustWorkerNode other)
        {
            if (trustVerifier == null)
            {
                trustVerifier = new UserTrustVerifier(user);
            }

            // todo:
            UserTrustProviderWorkerNode workerNode = (UserTrustProviderWorkerNode)other;

            return trustVerifier.GetTrust(workerNode.user.PublicKey.Span);
        }

        public UserInfo GetResult(Trust trust)
        {
            return UserInfo.FromDatabaseUser(user, trust);
        }

        public void Dispose()
        {
            trustVerifier?.Dispose();
        }
    }
}
