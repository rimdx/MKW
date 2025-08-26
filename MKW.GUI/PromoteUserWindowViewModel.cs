using MKW.GUI.Model;

namespace MKW.GUI
{
    public class PromoteUserWindowViewModel : ViewModelBase
    {
        private readonly DatabaseModel database;

        public PromoteUserWindowViewModel(DatabaseModel database)
        {
            this.database = database;
        }

        public string? Password { get; set; }
        public bool IsPasswordMatch { get; set; }

        public bool OnOK()
        {
            if (!IsPasswordMatch)
            {
                throw new Exception("Password and repeated password don't match.");
            }

            if (Password == null)
            {
                throw new ArgumentNullException(nameof(Password));
            }

            database.PromoteUser(Password);

            return true;
        }
    }
}
