using MKW.Core.Notify;

namespace MKW.Core
{
    public interface IAdminController : IDisposable
    {
        UserInfo CreateAdmin(string password, UserMetadata metadata);
        IUserSession OpenAdmin(string password);
        UserInfo GetAdminInfo();
    }
}
