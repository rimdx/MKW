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

        public string Password { get; set; } = "";
        public string PasswordRepeat { get; set; } = "";

        public bool PasswordsMatch => Password == PasswordRepeat;

        public bool OnOK() => RunAction(() =>
        {
            if (!PasswordsMatch)
            {
                throw new Exception("Password and repeated password don't match.");
            }

            database.PromoteUser(Password);
        });
    }
}
