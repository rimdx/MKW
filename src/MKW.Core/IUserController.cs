namespace MKW.Core
{
    public interface IUserController : IDisposable
    {
        IUserSession OpenUser(string password);
        IUserSession OpenUser(UserId id, string password);

        UserAccessRequest CreateUserAccessRequest(string password);

        IEnumerable<UserInfo> EnumerateUsers();
        UserInfo GetUserInfo(UserId id);
    }
}
