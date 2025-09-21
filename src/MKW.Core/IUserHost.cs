using MKW.Core.Notify;
using MKW.Core.Storage;

namespace MKW.Core
{
    public interface IUserHost : IDisposable
    {
        UserInfo CreateUser(UserAccessRequest request, UserMetadata metadata);

        void AddTrust(UserId userId);
    }
}
