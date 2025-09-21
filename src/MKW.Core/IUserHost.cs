namespace MKW.Core
{
    public interface IUserHost : IDisposable
    {
        UserInfo CreateUser(UserAccessRequest request, UserMetadata metadata);
    }
}
