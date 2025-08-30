using MKW.Core.Client.Notify;

namespace MKW.Core.Client
{
    public partial class ClientSession : IDisposable
    {
        public UserInfo PromoteAdmin(string password)
        {
            return adminController.PromoteAdmin(password);
        }

        public AdminSession OpenAdmin(string password)
        {
            return adminController.OpenAdmin(password);
        }

        public UserInfo GetAdminInfo()
        {
            return adminController.GetAdminInfo();
        }
    }
}
