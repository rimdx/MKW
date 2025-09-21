using MKW.Core.Notify;
using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core
{
    public interface IUserController : IDisposable
    {
        IUserSession OpenUser(string password);
        IUserSession OpenUser(UserId id, string password);
        IUserSession OpenUser(IDatabaseUser user, IUserCredentials creds);

        UserAccessRequest CreateUserAccessRequest(string password);

        IEnumerable<UserInfo> EnumerateUsers();
        UserInfo GetUserInfo(UserId id);
    }
}
