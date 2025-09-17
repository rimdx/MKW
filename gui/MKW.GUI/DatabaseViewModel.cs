using MKW.Core;
using MKW.GUI.Model;

namespace MKW.GUI
{
    public class DatabaseViewModel : ViewModelBase, IDisposable
    {
        public DatabaseModel Database { get; }

        public DatabaseViewModel(DatabaseModel database)
        {
            Database = database;

            Entries = new DatabaseEntryCollectionViewModel(database);
            Users = new DatabaseUserCollectionViewModel(database);
        }

        private enum PageType
        {
            Database,
            Entries,
            Users,
        }

        private PageType selectedPage = PageType.Entries;
        private void SetPage(bool value, PageType type)
        {
            if (value)
            {
                selectedPage = type;
                OnPropertyChanged(nameof(IsPageDatabase));
                OnPropertyChanged(nameof(IsPageEntries));
                OnPropertyChanged(nameof(IsPageUsers));
            }
        }

        public bool IsPageDatabase
        {
            get => selectedPage == PageType.Database;
            set => SetPage(value, PageType.Database);
        }

        public bool IsPageEntries
        {
            get => selectedPage == PageType.Entries;
            set => SetPage(value, PageType.Entries);
        }

        public bool IsPageUsers
        {
            get => selectedPage == PageType.Users;
            set => SetPage(value, PageType.Users);
        }

        public DatabaseEntryCollectionViewModel Entries { get; }

        private DatabaseEntryModel? _selectedEntry;
        public DatabaseEntryModel? SelectedEntry
        {
            get => _selectedEntry;
            set
            {
                SetProperty(ref _selectedEntry, value);
                OnPropertyChanged(nameof(IsEntrySelected));
            }
        }

        public bool IsEntrySelected => _selectedEntry != null;

        public DatabaseUserCollectionViewModel Users { get; }

        private DatabaseUserModel? _selectedUser;
        public DatabaseUserModel? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                SetProperty(ref _selectedUser, value);
                OnPropertyChanged(nameof(IsUserSelected));
                OnPropertyChanged(nameof(IsVerifiable));
            }
        }

        public bool IsUserSelected => _selectedUser != null;

        public bool IsVerifiable
        {
            get
            {
                if (Database.User != null && Database.User.Id.IsAdmin)
                {
                    if (SelectedUser != null)
                    {
                        return SelectedUser.IsVerifiable;
                    }
                }

                return false;
            }
        }

        // Entry

        public NewEntryWindowViewModel CreateNewEntryWindowViewModel()
        {
            return new NewEntryWindowViewModel(Database);
        }

        public EditEntryWindowViewModel CreateEditEntryWindowViewModel()
        {
            if (SelectedEntry == null)
            {
                throw new Exception("No entry was selected.");
            }

            IEntrySession entry = Database.User!.OpenEntry(SelectedEntry.Id);

            return new EditEntryWindowViewModel(Database, entry /* move */);
        }

        public bool DeleteEntry()
        {
            if (SelectedEntry == null)
            {
                throw new Exception("No entry was selected.");
            }

            Database.DeleteEntry(SelectedEntry.Id);

            return true;
        }

        // User

        public NewUserWindowViewModel CreateNewUserWindowViewModel()
        {
            return new NewUserWindowViewModel(Database);
        }

        public UserPropertyDialogViewModel CreateUserPropertiesWindowViewModel()
        {
            if (SelectedUser == null)
            {
                throw new Exception("No user was selected.");
            }

            UserEditorModel userEditor = Database.CreateUserEditor(SelectedUser.Id);

            return new UserPropertyDialogViewModel(Database, userEditor);
        }

        public void DeleteUser()
        {
            if (SelectedUser == null)
            {
                throw new Exception("No user was selected.");
            }

            Database.DeleteUser(SelectedUser.Id);
        }

        public void Dispose()
        {
            Entries.Dispose();
            Users.Dispose();
            Database.Dispose();
        }
    }
}
