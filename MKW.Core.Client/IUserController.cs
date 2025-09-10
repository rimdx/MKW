using MKW.Core.Client.Notify;
using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    public interface IUserController : IDisposable
    {
        UserInfo PromoteUser(string password);

        UserSession OpenUser(string password);
        UserSession OpenUser(UserId id, string password);
        UserSession OpenUser(IDatabaseUser user, IUserCredentials creds);

        IEnumerable<UserInfo> EnumerateUsers();
        UserInfo GetUserInfo(UserId id);
    }
}
