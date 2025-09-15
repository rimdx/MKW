using MKW.Core.Client.Notify;
using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    public interface IUserController : IDisposable
    {
        UserInfo PromoteUser(string password);

        IUserSession OpenUser(string password);
        IUserSession OpenUser(UserId id, string password);
        IUserSession OpenUser(IDatabaseUser user, IUserCredentials creds);

        IEnumerable<UserInfo> EnumerateUsers();
        UserInfo GetUserInfo(UserId id);
    }
}
