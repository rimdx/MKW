using MKW.Core.Notify;

namespace MKW.Core
{
    public interface IAdminController : IDisposable
    {
        UserInfo CreateAdmin(string password);
        IUserSession OpenAdmin(string password);
        UserInfo GetAdminInfo();
    }
}
