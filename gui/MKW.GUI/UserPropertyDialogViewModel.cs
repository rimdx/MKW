using MKW.Core.Notify;
using MKW.GUI.Model;
using MKW.GUI.Services;

namespace MKW.GUI
{
    public class UserPropertyDialogViewModel : ViewModelBase, IDisposable
    {
        private readonly DatabaseModel database;
        private readonly UserEditorModel user;
        private readonly KeyFormatter keyFormatter;

        public UserPropertyDialogViewModel(DatabaseModel database, UserEditorModel user /* move */)
        {
            this.database = database;
            this.user = user;
            keyFormatter = new KeyFormatter(50);
        }

        public string UserId => user.Id.ToString();

        public bool IsMe => user.Id == database.User?.Id;

        public bool OnOK()
        {
            user.OnApply();
            return true;
        }

        public bool OnVerify()
        {
            user.OnVerify();
            OnPropertyChanged(nameof(IsUntrusted));
            return true;
        }

        public string PublicKey => keyFormatter.GetString(user.PublicKey);

        public bool IsUntrusted => user.Trust == Trust.None;

        public bool IsUser => !database.User!.Id.IsAdmin;
        public bool IsAdmin => database.User!.Id.IsAdmin;

        public void Dispose()
        {
            user.Dispose();
        }
    }
}
