using MKW.GUI.CreateDatabaseWizard;
using MKW.GUI.Database;
using MKW.GUI.Model;
using MKW.GUI.Services;
using MKW.GUI.SingleInstance;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace MKW.GUI
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private readonly RegistryService registryService;
        private readonly RecentFilesService recentFilesService;
        private readonly AppModel appModel;
        private string title;

        public ObservableCollection<DatabaseTabItemViewModel> TabItems { get; }

        public MainWindowViewModel(AppModel appModel)
        {
            this.appModel = appModel;
            registryService = new RegistryService(RegistryKeys.RootKeyPath);
            recentFilesService = new RecentFilesService(registryService);
            RecentFiles = new RecentFilesCollectionViewModel(recentFilesService);
            TabItems = new ObservableCollection<DatabaseTabItemViewModel>();
            title = FormatTitle(selectedTab);

            ((INotifyPropertyChanged)TabItems).PropertyChanged += TabItems_PropertyChanged;
            TabItems_PropertyChanged(TabItems, new PropertyChangedEventArgs(null));

            try
            {
                string[] openFiles = registryService.GetOpenFiles();

                foreach (string file in openFiles)
                {
                    try
                    {
                        IDocumentLock document = appModel.OpenDatabase(file);
                        AddDatabaseTab(document);
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }

        private void TabItems_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            IsTabControlVisible = TabItems.Count > 0;
            IsStartPageVisible = TabItems.Count <= 0;
        }

        public RecentFilesCollectionViewModel RecentFiles { get; }

        public string Title
        {
            get => title;
            private set => SetProperty(ref title, value);
        }

        public CreateDatabaseWizardViewModel CreateCreateDatabaseViewModel()
        {
            return new CreateDatabaseWizardViewModel(this, registryService);
        }

        public DatabaseTabItemViewModel OpenDatabase(string databasePath)
        {
            DatabaseTabItemViewModel? tabViewModel;

            tabViewModel = GetDatabaseByPath(databasePath);
            if (tabViewModel == null)
            {
                IDocumentLock document = appModel.OpenDatabase(databasePath);

                tabViewModel = AddDatabaseTab(document);

                try
                {
                    UpdateOpenFilesList();
                }
                catch
                {
                }
            }

            SelectedTab = tabViewModel;

            recentFilesService.OnFileOpened(databasePath);

            return tabViewModel;
        }

        public void CreateDatabase(string databasePath, string password)
        {
            recentFilesService.OnFileOpened(databasePath);

            IDocumentLock document = appModel.CreateDatabase(databasePath, password);

            DatabaseTabItemViewModel tabViewModel = AddDatabaseTab(document);
            SelectedTab = tabViewModel;

            try
            {
                UpdateOpenFilesList();
            }
            catch
            {
            }
        }

        private void OpenDatabaseInternal(IDocumentLock document)
        {
            recentFilesService.OnFileOpened(document.Database.Path);

            DatabaseTabItemViewModel tabViewModel = AddDatabaseTab(document);
            SelectedTab = tabViewModel;

            try
            {
                UpdateOpenFilesList();
            }
            catch
            {
            }
        }

        private DatabaseTabItemViewModel? GetDatabaseByPath(string path)
        {
            string fullPath = Path.GetFullPath(path);

            foreach (DatabaseTabItemViewModel tabItem in TabItems)
            {
                if (string.Compare(Path.GetFullPath(tabItem.DatabaseViewModel.Database.Path), fullPath, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return tabItem;
                }
            }

            return null;
        }

        private DatabaseTabItemViewModel AddDatabaseTab(IDocumentLock document)
        {
            DatabaseTabItemViewModel tabViewModel = new DatabaseTabItemViewModel(new DatabaseViewModel(document));
            TabItems.Add(tabViewModel);

            return tabViewModel;
        }

        public void CloseTab(DatabaseTabItemViewModel selectedTab)
        {
            selectedTab.OnClose();
            TabItems.Remove(selectedTab);

            try
            {
                UpdateOpenFilesList();
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            registryService.Dispose();
            RecentFiles.Dispose();
        }

        private DatabaseTabItemViewModel? selectedTab;

        public DatabaseTabItemViewModel? SelectedTab
        {
            get => selectedTab;
            set
            {
                if (SetProperty(ref selectedTab, value))
                {
                    Title = FormatTitle(selectedTab);
                }
            }
        }

        private bool isStartPageVisible;

        public bool IsStartPageVisible
        {
            get => isStartPageVisible;
            set => SetProperty(ref isStartPageVisible, value);
        }

        private bool isTabControlVisible;

        public bool IsTabControlVisible
        {
            get => isTabControlVisible;
            set => SetProperty(ref isTabControlVisible, value);
        }

        private void UpdateOpenFilesList()
        {
            List<string> filesList = new List<string>();
            foreach (DatabaseTabItemViewModel tabItem in TabItems)
            {
                filesList.Add(tabItem.DatabaseViewModel.Database.Path);
            }

            registryService.SetOpenFiles(filesList.ToArray());
        }

        public void HandleRunRequest(RunRequest request)
        {
            foreach (string path in request.PathsToOpen)
            {
                try
                {
                    DatabaseTabItemViewModel? tabItem = GetDatabaseByPath(path);
                    if (tabItem != null)
                    {
                        SelectedTab = tabItem;
                    }
                    else
                    {
                        OpenDatabaseInternal(appModel.OpenDatabase(path));
                    }
                }
                catch
                {
                }
            }
        }

        private static string FormatTitle(DatabaseTabItemViewModel? selectedTab)
        {
            if (selectedTab == null)
            {
                return "Multi-Key Wallet";
            }
            else
            {
                return $"{Path.GetFileName(selectedTab.DatabaseViewModel.Database.Path)} - Multi-Key Wallet";
            }
        }
    }
}
