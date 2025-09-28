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
            set
            {
                if (value == null)
                {
                    return;
                }

                if (SetProperty(ref selectedUser, value))
                {
                    Profile = new LoginProfileViewModel(this, selectedUser);
                }
            }
        }

        private LoginProfileViewModel profile;
        public LoginProfileViewModel Profile
        {
            get => profile;
            set => SetProperty(ref profile, value);
        }

        public DatabaseLockedViewModel(DatabaseViewModel database)
        {
            Database = database;
            Users = new DatabaseUserCollectionViewModel(database.Database);

            selectedUser = Users[0];
            profile = new LoginProfileViewModel(this, selectedUser);
        }
    }
}
