using MKW.GUI.Model;
using System.Collections.ObjectModel;

namespace MKW.GUI
{
    public class LoginWindowViewModel : ViewModelBase
    {
        private readonly DatabaseModel database;

        public LoginWindowViewModel(DatabaseModel database)
        {
            this.database = database;

            Users = [];

            LoadUsers();
        }

        private void LoadUsers()
        {
            Users.Clear();

            foreach (DatabaseUserModel user in database.EnumerateUsers())
            {
                Users.Add(user);
            }

            SelectedUser = Users[0];
        }

        public ObservableCollection<DatabaseUserModel> Users { get; }
        public DatabaseUserModel? SelectedUser { get; set; }

        public string Password { get; set; } = "";

        public string DatabasePath => database.Path;

        public bool DoLogin()
        {
            if (SelectedUser == null)
            {
                throw new Exception("Please select user.");
            }

            database.Authenticate(SelectedUser.Id, Password);

            return true;
        }
    }
}
