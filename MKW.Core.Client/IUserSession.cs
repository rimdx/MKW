using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public interface IUserSession : IEntryController, ITrustProvider, ITrustController, IDisposable
    {
        UserId Id { get; }
    }
}
