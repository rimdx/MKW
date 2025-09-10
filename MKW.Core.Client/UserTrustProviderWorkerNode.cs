using MKW.Core.Client.Notify;
using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    internal class UserTrustProviderWorkerNode : ITrustWorkerNode, IDisposable
    {
        private readonly ICryptographyProvider crypto;
        private readonly IDatabaseUser user;
        private ITrustVerifier? trustVerifier;

        public UserId Id => user.Id;

        public UserTrustProviderWorkerNode(ICryptographyProvider crypto, IDatabaseUser user)
        {
            this.crypto = crypto;
            this.user = user;
        }

        public Trust GetTrust(ITrustWorkerNode other)
        {
            if (trustVerifier == null)
            {
                trustVerifier = new UserTrustVerifier(crypto,  user);
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
