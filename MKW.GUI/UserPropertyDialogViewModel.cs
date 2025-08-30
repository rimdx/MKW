using MKW.Core.Client.Notify;
using MKW.GUI.Model;
using MKW.GUI.Services;

namespace MKW.GUI
{
    public class UserPropertyDialogViewModel : ViewModelBase
    {
        private readonly DatabaseModel database;
        private readonly UserInfo user;
        private readonly KeyFormatter keyFormatter;
        private readonly Trust trust;

        public UserPropertyDialogViewModel(DatabaseModel database, UserInfo user /* reference */)
        {
            this.database = database;
            this.user = user;
            keyFormatter = new KeyFormatter(50);

            trust = Trust.None;
            foreach (UserInfo userTrust in database.User!.EnumerateUsersTrust())
            {
                if (userTrust.Id == user.Id)
                {
                    trust = userTrust.Trust;
                }
            }
        }

        public string UserId => user.Id.ToString();

        public bool IsMe => user.Id == database.User?.Id;

        public bool OnOK()
        {
            return true;
        }

        public string PublicKey => keyFormatter.GetString(user.PublicKey.Span);

        public bool IsUntrusted => trust == Trust.None;
        public bool IsUser => !database.User!.Id.IsAdmin;
        public bool IsAdmin => database.User!.Id.IsAdmin;
    }
}
