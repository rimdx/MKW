namespace MKW.Core
{
    public interface IAdminSession
        : IUserSession
        , IEntryController
        , ITrustProvider
        , IDisposable
    {
        UserInfo CreateUser(UserAccessRequest request, UserMetadata metadata);
    }
}
