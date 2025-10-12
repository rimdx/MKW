namespace MKW.Core
{
    public interface IAdminSession
        : IUserSession
        , IDisposable
    {
        UserInfo CreateUser(UserAccessRequest request, UserMetadata metadata);
    }
}
