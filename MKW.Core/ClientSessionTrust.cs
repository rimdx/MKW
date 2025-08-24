using MKW.Core.Client.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public partial class ClientSession : IDisposable
    {
        public IEnumerable<UserInfo> EnumerateUsersTrust()
        {
            IDatabaseUser admin = Database.OpenAdmin(true);
            UserTrustProvider trust = new UserTrustProvider(this, admin);

            return trust.EnumerateUsersTrust();
        }
    }
}
