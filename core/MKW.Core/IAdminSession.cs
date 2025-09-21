namespace MKW.Core
{
    public interface IAdminSession
        : IUserSession
        , IEntryController
        , ITrustProvider
        , ITrustController
        , IDisposable
    {
    }
}
