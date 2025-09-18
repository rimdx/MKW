using MKW.GUI.Model;
using MKW.GUI.Services;
using System.ComponentModel;

namespace MKW.GUI
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private readonly RegistryService registryService;
        private readonly RecentFilesService recentFilesService;

        public MainWindowViewModel()
        {
            registryService = new RegistryService(RegistryKeys.RootKeyPath);
            recentFilesService = new RecentFilesService(registryService);
            RecentFiles = new RecentFilesCollectionViewModel(recentFilesService);
        }

        public RecentFilesCollectionViewModel RecentFiles { get; }

        public string Title => "Multi-Key Wallet";
        public string Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!.ToString();

        private DatabaseViewModel? _database;
        public DatabaseViewModel? Database
        {
            get => _database;
            set
            {
                _database?.Dispose();
                SetProperty(ref _database, value);
                OnPropertyChanged(nameof(IsDatabaseAttached));
                OnPropertyChanged(nameof(IsEntrySelected));

                if (_database != null)
                {
                    _database.PropertyChanged += Database_PropertyChanged;
                }
            }
        }

        private void Database_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.MatchProperty(nameof(_database.SelectedEntry)))
            {
                OnPropertyChanged(nameof(SelectedEntry));
                OnPropertyChanged(nameof(IsEntrySelected));
            }

            if (e.MatchProperty(nameof(_database.SelectedEntry)))
            {
                OnPropertyChanged(nameof(SelectedUser));
                OnPropertyChanged(nameof(IsUserSelected));
            }
        }

        public bool IsDatabaseAttached => _database != null;

        public DatabaseEntryModel? SelectedEntry => _database?.SelectedEntry;
        public bool IsEntrySelected => SelectedEntry != null;

        public DatabaseUserModel? SelectedUser => _database?.SelectedUser;
        public bool IsUserSelected => SelectedUser != null;

        public DatabaseModel GetDatabase()
        {
            if (_database == null)
            {
                throw new Exception("No database is attached.");
            }

            return _database.Database;
        }

        public CreateDatabaseWindowViewModel CreateCreateDatabaseViewModel()
        {
            return new CreateDatabaseWindowViewModel();
        }

        public LoginWindowViewModel CreateLoginViewModel(string filename)
        {
            DatabaseModel database = DatabaseModel.Open(filename);
            return new LoginWindowViewModel(database /* move */);
        }

        public void OpenDatabase(CreateDatabaseWindowViewModel createDatabaseViewModel)
        {
            if (createDatabaseViewModel.Database != null)
            {
                recentFilesService.OnFileOpened(createDatabaseViewModel.Database.Path);
                Database = new DatabaseViewModel(createDatabaseViewModel.Database /* move */);
            }
            else
            {
                /* no-op */
            }
        }

        public void OpenDatabase(LoginWindowViewModel loginWindowViewModel)
        {
            recentFilesService.OnFileOpened(loginWindowViewModel.Database.Path);

            if (loginWindowViewModel.Database.User == null)
            {
                loginWindowViewModel.Database.Dispose();
            }
            else
            {
                Database = new DatabaseViewModel(loginWindowViewModel.Database /* move */);
            }
        }

        public void OnCloseDatabase()
        {
            Database = null;
        }

        public void Dispose()
        {
            Database?.Dispose();
            registryService.Dispose();
            RecentFiles.Dispose();
        }
    }
}
