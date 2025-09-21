namespace MKW.Core
{
    public interface IAdminSession
        : IUserSession
        , IUserHost
        , IEntryController
        , ITrustProvider
        , IDisposable
    {
    }
}
