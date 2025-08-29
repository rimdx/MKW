using MKW.Core.Client.Notify;
using MKW.GUI.Model;
using MKW.GUI.Services;

namespace MKW.GUI
{
    public class UserPropertyDialogViewModel : ViewModelBase
    {
        private readonly UserInfo user;
        private readonly KeyFormatter keyFormatter;

        public UserPropertyDialogViewModel(DatabaseModel database, UserInfo user /* reference */)
        {
            this.user = user;
            keyFormatter = new KeyFormatter(50);
            TrustedUsers = new TrustedUsersCollectionViewModel(database, user.Id);
        }

        public string UserId => user.Id.ToString();

        public TrustedUsersCollectionViewModel TrustedUsers { get; }

        public bool OnOK()
        {
            return true;
        }

        public string PublicKey => keyFormatter.GetString(user.PublicKey.Span);
    }
}
