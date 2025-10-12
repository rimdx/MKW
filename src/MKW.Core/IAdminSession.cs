namespace MKW.Core
{
    public interface IAdminSession
        : IUserSession
        , ITrustProvider
        , IDisposable
    {
        UserInfo CreateUser(UserAccessRequest request, UserMetadata metadata);
    }
}
