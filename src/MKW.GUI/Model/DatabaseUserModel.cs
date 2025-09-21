using MKW.Core;
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
        public ImageMoniker Icon => IsAdmin ? ImageMoniker.Admin : ImageMoniker.User;

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
            Trust.SelfTrust => ImageMoniker.StatusOK,
            Trust.ExplicitTrust => ImageMoniker.StatusOK,
            Trust.ImplicitTrust => ImageMoniker.StatusOK,
            Trust.None => ImageMoniker.StatusWarning,
            Trust.Unknown => ImageMoniker.StatusWarning,
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
