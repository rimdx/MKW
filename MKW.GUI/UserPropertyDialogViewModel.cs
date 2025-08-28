using MKW.Core.Client.Notify;
using MKW.GUI.Model;

namespace MKW.GUI
{
    public class UserPropertyDialogViewModel : ViewModelBase
    {
        private readonly UserInfo user;

        public UserPropertyDialogViewModel(DatabaseModel database, UserInfo user /* reference */)
        {
            this.user = user;
        }

        public string UserId => user.Id.ToString();

        public bool OnOK()
        {
            return true;
        }
    }
}
