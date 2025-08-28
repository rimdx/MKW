using MKW.Core.Client;
using MKW.GUI.Model;
using System.Collections.ObjectModel;

namespace MKW.GUI
{
    public class DatabaseViewModel : ViewModelBase, IDisposable
    {
        public DatabaseModel Database { get; }

        public DatabaseViewModel(DatabaseModel database)
        {
            Database = database;

            Entries = new DatabaseEntryCollectionViewModel(database);

            Database.OnUsersChanged += Database_OnUsersChanged;
            Users = [];
            RefreshUsers();
        }

        private void Database_OnUsersChanged(object? sender, EventArgs e)
        {
            RefreshUsers();
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

        public ObservableCollection<DatabaseUserModel> Users { get; private set; }

        private DatabaseUserModel? _selectedUser;
        public DatabaseUserModel? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                SetProperty(ref _selectedUser, value);
                OnPropertyChanged(nameof(IsUserSelected));
            }
        }

        public bool IsUserSelected => _selectedUser != null;

        public void RefreshUsers()
        {
            Users.Clear();

            foreach (DatabaseUserModel user in Database.EnumerateUsers())
            {
                Users.Add(user);
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

            UserEntry entry = Database.User!.OpenEntry(SelectedEntry.Id);

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

        public PromoteUserWindowViewModel CreatePromoteUserWindowViewModel()
        {
            return new PromoteUserWindowViewModel(Database);
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
