using MKW.Core.Client.Notify;
using MKW.Core.Storage;
using MKW.GUI.Images;

namespace MKW.GUI
{
    public class TrustedUserItemViewModel
    {
        private readonly UserInfo user;

        public UserId UserId => user.Id;

        public object Icon
        {
            get
            {
                if (user.Id.IsAdmin)
                    return new Admin();
                else
                    return new User();
            }
        }

        public TrustedUserItemViewModel(UserInfo user)
        {
            this.user = user;
        }
    }
}
