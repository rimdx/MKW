using MKW.GUI.CreateDatabaseWizard;
using MKW.GUI.Database;
using MKW.GUI.Images;
using MKW.GUI.Model;
using MKW.GUI.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;

namespace MKW.GUI
{
    public class DatabaseTabItemViewModel : ViewModelBase
    {
        private readonly DatabaseViewModel databaseViewModel;
        private ImageMoniker icon;

        public string Header { get; }
        public string Tooltip { get; }
        public ContentControl Content { get; }
        public ImageMoniker Icon
        {
            get => icon;
            private set => SetProperty(ref icon, value);
        }

        public DatabaseTabItemViewModel(DatabaseViewModel databaseViewModel)
        {
            this.databaseViewModel = databaseViewModel;

            Header = Path.GetFileNameWithoutExtension(databaseViewModel.Database.Path);
            Tooltip = databaseViewModel.Database.Path;
            Content = new DatabasePage(databaseViewModel);
            Icon = GetIcon(databaseViewModel.Database);

            databaseViewModel.Database.PropertyChanged += Database_PropertyChanged;
        }

        private void Database_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.MatchProperty(nameof(DatabaseModel.User)))
            {
                Icon = GetIcon(databaseViewModel.Database);
            }
        }

        private static ImageMoniker GetIcon(DatabaseModel database)
        {
            if (database.User == null)
            {
                return ImageMoniker.ReadOnlyDatabase;
            }
            else
            {
                return ImageMoniker.Database;
            }
        }

        public DatabaseViewModel DatabaseViewModel => databaseViewModel;

        public void OnClose()
        {
            databaseViewModel.Dispose();
        }
    }

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
            return new CreateDatabaseWizardViewModel(appModel, registryService);
        }

        public LoginWindowViewModel CreateLoginViewModel(string filename)
        {
            DatabaseModel database = appModel.OpenDatabase(filename);
            return new LoginWindowViewModel(database /* move */);
        }

        public void OpenDatabase(CreateDatabaseWizardViewModel createDatabaseViewModel)
        {
            if (createDatabaseViewModel.Database != null)
            {
                recentFilesService.OnFileOpened(createDatabaseViewModel.Database.Path);
                DatabaseTabItemViewModel tabViewModel = new DatabaseTabItemViewModel(new DatabaseViewModel(createDatabaseViewModel.Database /* move */));
                TabItems.Add(tabViewModel);
                SelectedTab = tabViewModel;
            }
            else
            {
                /* no-op */
            }

            try
            {
                UpdateOpenFilesList();
            }
            catch
            {
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
                DatabaseTabItemViewModel tabViewModel = new DatabaseTabItemViewModel(new DatabaseViewModel(loginWindowViewModel.Database /* move */));
                TabItems.Add(tabViewModel);
                SelectedTab = tabViewModel;
            }

            try
            {
                UpdateOpenFilesList();
            }
            catch
            {
            }
        }

        private DatabaseTabItemViewModel AddDatabaseTab(DatabaseModel database)
        {
            DatabaseTabItemViewModel tabViewModel = new DatabaseTabItemViewModel(new DatabaseViewModel(database));
            TabItems.Add(tabViewModel);

            return tabViewModel;
        }

        public void OnCloseTab(DatabaseTabItemViewModel selectedTab)
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
    }
}
