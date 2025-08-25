using MKW.Core.Client;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MKW.GUI
{
    public class DatabaseViewModel : INotifyPropertyChanged, IDisposable
    {
        public DatabaseModel Database { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public DatabaseViewModel(DatabaseModel database)
        {
            Database = database;
            Database.OnEntriesChanged += Database_OnEntriesChanged;

            Entries = [];
            RefreshEntries();

            Users = [];
            RefreshUsers();
        }

        private void Database_OnEntriesChanged(object? sender, EventArgs e)
        {
            RefreshEntries();
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

        public ObservableCollection<DatabaseEntryModel> Entries { get; private set; }

        private DatabaseEntryModel? _selectedEntry;
        public DatabaseEntryModel? SelectedEntry
        {
            get => _selectedEntry;
            set
            {
                _selectedEntry = value;
                OnPropertyChanged(nameof(SelectedEntry));
                OnPropertyChanged(nameof(IsEntrySelected));
            }
        }

        public bool IsEntrySelected => _selectedEntry != null;

        public void RefreshEntries()
        {
            Entries.Clear();

            foreach (UserEntry entry in Database.User!.EnumerateEntries())
            {
                Entries.Add(new DatabaseEntryModel
                {
                    Id = entry.Id,
                    Payload = entry.OpenPayload()?.ToString()
                });
            }
        }

        public ObservableCollection<LoginUser> Users { get; private set; }

        private LoginUser? _selectedUser;
        public LoginUser? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
                OnPropertyChanged(nameof(IsUserSelected));
            }
        }

        public bool IsUserSelected => _selectedUser != null;

        public void RefreshUsers()
        {
            Users.Clear();

            foreach (LoginUser user in Database.EnumerateUsers())
            {
                Users.Add(user);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void OnAddEntry()
        {
            NewEntryWindow window = new NewEntryWindow(Database);
            window.ShowDialog();
        }

        public void OnEditEntry()
        {
            if (SelectedEntry == null)
            {
                throw new Exception("No entry was selected.");
            }

            using UserEntry entry = Database.User!.OpenEntry(SelectedEntry.Id);

            EditEntryWindow window = new EditEntryWindow(Database,
                                                         entry /* reference */);

            window.ShowDialog();
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
