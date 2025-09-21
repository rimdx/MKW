using MKW.Core.Notify;

namespace MKW.Core
{
    public interface IAdminController : IDisposable
    {
        UserInfo CreateAdmin(string password, UserMetadata metadata);
        IAdminSession OpenAdmin(string password);
        UserInfo GetAdminInfo();
    }
}
