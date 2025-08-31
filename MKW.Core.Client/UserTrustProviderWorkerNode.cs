using MKW.Core.Client.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    internal class UserTrustProviderWorkerNode : ITrustWorkerNode, IDisposable
    {
        private readonly ClientSession client;
        private readonly IDatabaseUser user;
        private UserTrustProvider? trustProvider;

        public UserId Id => user.Id;

        public UserTrustProviderWorkerNode(ClientSession client, IDatabaseUser user)
        {
            this.client = client;
            this.user = user;
        }

        public Trust GetTrust(ITrustWorkerNode other)
        {
            if (trustProvider == null)
            {
                trustProvider = new UserTrustProvider(client, user);
            }

            // todo:
            UserTrustProviderWorkerNode workerNode = (UserTrustProviderWorkerNode)other;

            return trustProvider.GetExplicitTrust(workerNode.user.PublicKey.Span);
        }

        public UserInfo GetResult(Trust trust)
        {
            return UserInfo.FromDatabaseUser(user, trust);
        }

        public void Dispose()
        {
            trustProvider?.Dispose();
        }
    }
}
