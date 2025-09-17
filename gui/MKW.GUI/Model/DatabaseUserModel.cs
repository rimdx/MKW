using MKW.Core.Notify;
using MKW.Core.Storage;
using MKW.GUI.Images;

namespace MKW.GUI.Model
{
    public class DatabaseUserModel
    {
        private readonly UserInfo user;
        private readonly Trust trust;

        public DatabaseUserModel(UserInfo user, Trust trust)
        {
            this.user = user;
            this.trust = trust;
        }

        public UserId Id => user.Id;
        public bool IsAdmin => Id.IsAdmin;
        public string Name => IsAdmin ? "Admin" : "User";
        public object Icon => IsAdmin ? new Admin() : new User();

        public string StatusText => trust switch
        {
            Trust.SelfTrust => "Verified",
            Trust.ExplicitTrust => "Verified",
            Trust.ImplicitTrust => "Verified",
            Trust.None => "Unverified",
            Trust.Unknown => "Unknown",
        };

        public object StatusIcon => trust switch
        {
            Trust.SelfTrust => new StatusOK(),
            Trust.ExplicitTrust => new StatusOK(),
            Trust.ImplicitTrust => new StatusOK(),
            Trust.None => new StatusWarning(),
            Trust.Unknown => new StatusWarning(),
        };

        public bool IsVerifiable => trust switch
        {
            Trust.SelfTrust => false,
            Trust.ExplicitTrust => false,
            Trust.ImplicitTrust => false,
            Trust.None => true,
            Trust.Unknown => false,
        };
    }
}
