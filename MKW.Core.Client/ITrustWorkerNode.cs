using MKW.Core.Client.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    internal interface ITrustWorkerNode : IDisposable
    {
        UserId Id { get; }

        Trust GetTrust(ITrustWorkerNode other);
        UserInfo GetResult(Trust trust);
    }
}
