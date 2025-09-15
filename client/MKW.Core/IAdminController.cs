using MKW.Core.Notify;

namespace MKW.Core
{
    public interface IAdminController : IDisposable
    {
        UserInfo PromoteAdmin(string password);
        IUserSession OpenAdmin(string password);
        UserInfo GetAdminInfo();
    }
}
