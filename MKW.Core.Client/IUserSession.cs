using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public interface IUserSession : IEntryController, IDisposable
    {
        UserId Id { get; }
    }
}
