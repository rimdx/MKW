using MKW.GUI.Model;

namespace MKW.GUI.Database
{
    public class LoginProfileViewModel : ViewModelBase
    {
        private readonly DatabaseLockedViewModel database;
        private readonly DatabaseUserModel user;

        public LoginProfileViewModel(DatabaseLockedViewModel database, DatabaseUserModel user)
        {
            this.database = database;
            this.user = user;
        }

        private string password = "";
        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        public void Login()
        {
            database.Database.Database.Unlock(user.Id, Password);
        }

        public string LoginUserName => user.LoginUserName;
    }
}
