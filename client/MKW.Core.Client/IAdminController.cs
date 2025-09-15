using MKW.Core.Client.Notify;

namespace MKW.Core.Client
{
    public interface IAdminController : IDisposable
    {
        UserInfo PromoteAdmin(string password);
        IUserSession OpenAdmin(string password);
        UserInfo GetAdminInfo();
    }
}
