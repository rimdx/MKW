using MKW.GUI.Model;

namespace MKW.GUI.Database
{
    public class DatabaseLockedViewModel : ViewModelBase
    {
        public DatabaseViewModel Database { get; }

        public DatabaseUserCollectionViewModel Users { get; }

        private DatabaseUserModel selectedUser;
        public DatabaseUserModel SelectedUser
        {
            get => selectedUser;
            set => SetProperty(ref selectedUser, value);
        }

        private string password = "";
        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        public DatabaseLockedViewModel(DatabaseViewModel database)
        {
            Database = database;
            Users = new DatabaseUserCollectionViewModel(database.Database);
            selectedUser = Users[0];
        }

        public void Login()
        {
            Database.Database.Unlock(SelectedUser.Id, Password);
        }
    }
}
