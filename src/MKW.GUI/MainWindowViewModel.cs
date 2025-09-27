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

        public ObservableCollection<DatabaseTabItemViewModel> TabItems { get; }

        public MainWindowViewModel(AppModel appModel)
        {
            this.appModel = appModel;
            registryService = new RegistryService(RegistryKeys.RootKeyPath);
            recentFilesService = new RecentFilesService(registryService);
            RecentFiles = new RecentFilesCollectionViewModel(recentFilesService);
            TabItems = new ObservableCollection<DatabaseTabItemViewModel>();

            ((INotifyPropertyChanged)TabItems).PropertyChanged += TabItems_PropertyChanged;
            TabItems_PropertyChanged(TabItems, new PropertyChangedEventArgs(null));

            try
            {
                string[] openFiles = registryService.GetOpenFiles();

                foreach (string file in openFiles)
                {
                    try
                    {
                        DatabaseModel databaseModel = appModel.OpenDatabase(file);
                        AddDatabaseTab(databaseModel);
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
            IsTabControlVisible = (TabItems.Count > 0);
            IsStartPageVisible = (TabItems.Count <= 0);
        }

        public RecentFilesCollectionViewModel RecentFiles { get; }

        public string Title => "Multi-Key Wallet";
        public string Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!.ToString();

        public CreateDatabaseWizardViewModel CreateCreateDatabaseViewModel()
        {
            return new CreateDatabaseWizardViewModel(this, registryService);
        }

        public DatabaseTabItemViewModel OpenDatabase(string databasePath)
        {
            recentFilesService.OnFileOpened(databasePath);

            DatabaseModel database = appModel.OpenDatabase(databasePath);

            DatabaseTabItemViewModel tabViewModel = AddDatabaseTab(database);
            SelectedTab = tabViewModel;

            try
            {
                UpdateOpenFilesList();
            }
            catch
            {
            }

            return tabViewModel;
        }

        public void CreateDatabase(string databasePath, string password)
        {
            recentFilesService.OnFileOpened(databasePath);

            DatabaseModel database = appModel.CreateDatabase(databasePath, password);

            DatabaseTabItemViewModel tabViewModel = AddDatabaseTab(database);
            SelectedTab = tabViewModel;

            try
            {
                UpdateOpenFilesList();
            }
            catch
            {
            }
        }

        private void OpenDatabaseInternal(DatabaseModel database)
        {
            recentFilesService.OnFileOpened(database.Path);

            DatabaseTabItemViewModel tabViewModel = AddDatabaseTab(database);
            SelectedTab = tabViewModel;

            try
            {
                UpdateOpenFilesList();
            }
            catch
            {
            }
        }

        public DatabaseTabItemViewModel? GetDatabaseByPath(string path)
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

        private DatabaseTabItemViewModel AddDatabaseTab(DatabaseModel database)
        {
            DatabaseTabItemViewModel tabViewModel = new DatabaseTabItemViewModel(new DatabaseViewModel(database));
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
            set => SetProperty(ref selectedTab, value); 
        }

        private bool isStartPageVisible;

        public bool IsStartPageVisible
        { 
            get => isStartPageVisible; 
            set => SetProperty(ref isStartPageVisible, value); 
        }

        private bool  isTabControlVisible;

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
    }
}
