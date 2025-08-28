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
            TrustedUsers = new TrustedUsersCollectionViewModel(database, user.Id);
        }

        public string UserId => user.Id.ToString();

        public TrustedUsersCollectionViewModel TrustedUsers { get; }

        public bool OnOK()
        {
            return true;
        }
    }
}
