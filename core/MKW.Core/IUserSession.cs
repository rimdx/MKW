using MKW.Core.Storage;

namespace MKW.Core
{
    public interface IUserSession
        : IEntryController
        , ITrustProvider
        , ITrustController
        , IDisposable
    {
        UserId Id { get; }
    }
}
