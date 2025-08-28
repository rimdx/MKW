using Microsoft.Win32;
using MKW.GUI.Model;
using System.ComponentModel;

namespace MKW.GUI
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private readonly RegistryService registryService;
        private readonly RecentFilesService recentFilesService;

        public MainWindowViewModel()
        {
            registryService = new RegistryService();
            recentFilesService = new RecentFilesService(registryService);
            RecentFiles = new RecentFilesCollectionViewModel(recentFilesService);
        }

        public RecentFilesCollectionViewModel RecentFiles { get; }

        public string Title => "Multi-Key Wallet";

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
            if (e.PropertyName == nameof(_database.SelectedEntry))
            {
                OnPropertyChanged(nameof(SelectedEntry));
                OnPropertyChanged(nameof(IsEntrySelected));
            }
        }

        public bool IsDatabaseAttached => _database != null;

        public DatabaseEntryModel? SelectedEntry => _database?.SelectedEntry;
        public bool IsEntrySelected => SelectedEntry != null;

        public DatabaseModel GetDatabase()
        {
            if (_database == null)
            {
                throw new Exception("No database is attached.");
            }

            return _database.Database;
        }

        public bool OnNewDatabase() => RunAction(() =>
        {
            FileDialog dialog = new SaveFileDialog
            {
                FileName = "New Database",
                DefaultExt = ".mkw",
                Filter = "Multi-Key Wallet Database File|*.mkw"
            };

            if (dialog.ShowDialog() == true)
            {
                recentFilesService.OnFileOpened(dialog.FileName);

                CreateDatabaseWindowViewModel createDatabaseViewModel =
                    new CreateDatabaseWindowViewModel(dialog.FileName);
                CreateDatabaseWindow createDatabaseWindow =
                    new CreateDatabaseWindow(createDatabaseViewModel);

                createDatabaseWindow.ShowDialog();

                if (createDatabaseViewModel.Database != null)
                {
                    Database = new DatabaseViewModel(createDatabaseViewModel.Database /* move */);
                }
            }
        });

        public void OnOpenDatabase() => RunAction(() =>
        {
            FileDialog dialog = new OpenFileDialog
            {
                DefaultExt = ".mkw",
                Filter = "Multi-Key Wallet Database File|*.mkw"
            };

            if (dialog.ShowDialog() == true)
            {
                recentFilesService.OnFileOpened(dialog.FileName);

                DatabaseModel database = DatabaseModel.Open(dialog.FileName);
                LoginWindow window = new LoginWindow(database);

                window.ShowDialog();

                if (database.User == null)
                {
                    database.Dispose();
                }
                else
                {
                    Database = new DatabaseViewModel(database /* move */);
                }
            }
        });

        public bool OnCloseDatabase() => RunAction(() =>
        {
            Database = null;
        });

        public bool OnLockWorkspace() => RunAction(() =>
        {
            throw new NotImplementedException();
        });

        public bool OnSave() => RunAction(() =>
        {
            throw new NotImplementedException();
        });

        public bool OnSaveAs() => RunAction(() =>
        {
            throw new NotImplementedException();
        });

        // Help

        public void OnHelpAbout() => RunAction(() =>
        {
            throw new NotImplementedException();
        });

        public void Dispose()
        {
            Database?.Dispose();
            registryService.Dispose();
            RecentFiles.Dispose();
        }
    }
}
