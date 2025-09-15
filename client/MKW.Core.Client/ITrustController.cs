using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public interface ITrustController : IDisposable
    {
        void AddTrust(UserId userId);
        void RemoveTrust(UserId userId);
    }
}
