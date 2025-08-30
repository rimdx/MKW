using MKW.Core.Client.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public interface ITrustController : IDisposable
    {
        void UpdateTrust(UserId userId, Trust trust);
    }
}
