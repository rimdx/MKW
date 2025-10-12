using MKW.Core;
using MKW.GUI.AddUserWizard;
using MKW.GUI.EntryEditor;
using MKW.GUI.ExportWizard;
using MKW.GUI.ImportWizard;
using MKW.GUI.Model;
using MKW.GUI.RequestAccessWizard;

namespace MKW.GUI.Database
{
    public class DatabaseUnlockedViewModel : ViewModelBase
    {
        private readonly DatabaseUnlockedModel database;

        public DatabaseUnlockedViewModel(DatabaseUnlockedModel database)
        {
            this.database = database;

            Entries = new DatabaseEntryCollectionViewModel(database);
            Users = new DatabaseUserCollectionViewModel(database.Database);
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

        private EntryEditorModel? _selectedEntry;
        public EntryEditorModel? SelectedEntry
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
            }
        }

        public bool IsUserSelected => _selectedUser != null;

        // Entry

        public NewEntryWindowViewModel CreateNewEntryWindowViewModel()
        {
            return new NewEntryWindowViewModel(database);
        }

        public EditEntryWindowViewModel CreateEditEntryWindowViewModel(EntryId id)
        {
            return new EditEntryWindowViewModel(database, database.OpenEntry(id));
        }

        public void DeleteEntry(EntryEditorModel entry)
        {
            database.DeleteEntry(entry.Id);
        }

        // User

        public AddUserWizardViewModel CreateNewUserWindowViewModel()
        {
            return new AddUserWizardViewModel(database);
        }

        public RequestAccessWizardViewModel CreateRequestAccessViewModel()
        {
            return new RequestAccessWizardViewModel(database.Database);
        }

        public UserPropertyDialogViewModel CreateUserPropertiesWindowViewModel()
        {
            if (SelectedUser == null)
            {
                throw new Exception("No user was selected.");
            }

            UserEditorModel userEditor = database.CreateUserEditor(SelectedUser.Id);

            return new UserPropertyDialogViewModel(database, userEditor);
        }

        public void DeleteUser()
        {
            if (SelectedUser == null)
            {
                throw new Exception("No user was selected.");
            }

            database.DeleteUser(SelectedUser.Id);
        }

        public ImportWizardViewModel CreateImportViewModel()
        {
            return new ImportWizardViewModel(database);
        }

        public ExportWizardViewModel CreateExportViewModel()
        {
            return new ExportWizardViewModel(database);
        }

        private DatabaseUnlockedView idontcareifitsactuallywrong;
        public DatabaseUnlockedView UnlockedViewNeededToBindTheCommand
        {
            get => idontcareifitsactuallywrong;
            set => SetProperty(ref idontcareifitsactuallywrong, value);
        }

        public void Dispose()
        {
            Entries.Dispose();
            Users.Dispose();
        }
    }
}
