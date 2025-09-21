using MKW.GUI.Model;
using MKW.GUI.RequestAccessWizard;
using System.Collections.ObjectModel;

namespace MKW.GUI
{
    public class LoginWindowViewModel : ViewModelBase
    {
        public DatabaseModel Database { get; private set; }

        public LoginWindowViewModel(DatabaseModel database)
        {
            Database = database;

            Users = [];

            LoadUsers();
        }

        private void LoadUsers()
        {
            Users.Clear();

            foreach (DatabaseUserModel user in Database.EnumerateUsers())
            {
                Users.Add(user);
            }

            SelectedUser = Users[0];
        }

        public ObservableCollection<DatabaseUserModel> Users { get; }
        public DatabaseUserModel? SelectedUser { get; set; }

        public string Password { get; set; } = "";

        public string DatabasePath => Database.Path;

        public bool DoLogin()
        {
            if (SelectedUser == null)
            {
                throw new Exception("Please select user.");
            }

            Database.Unlock(SelectedUser.Id, Password);

            return true;
        }

        public RequestAccessWizardViewModel CreateRequestAccessViewModel()
        {
            return new RequestAccessWizardViewModel(Database);
        }
    }
}
