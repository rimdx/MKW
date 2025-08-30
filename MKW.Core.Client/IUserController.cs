using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public interface IUserController : IDisposable
    {
        UserInfo PromoteUser(string password);

        UserSession OpenUser(string password);
        UserSession OpenUser(UserId id, string password);
        UserSession OpenUser(IDatabaseUser user, UserCredentials creds);
    }
}
