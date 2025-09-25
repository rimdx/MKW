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

        public string PublicKey => keyFormatter.GetBase32String(user.PublicKey);

        public bool OnOK()
        {
            user.OnApply();
            return true;
        }

        public void Dispose()
        {
            user.Dispose();
        }
    }
}
