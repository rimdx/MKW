using MKW.Core.Storage;

namespace MKW.Core
{
    public interface ITrustController : IDisposable
    {
        void AddTrust(UserId userId);
        void RemoveTrust(UserId userId);
    }
}
