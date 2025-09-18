using MKW.GUI.Model;
using MKW.GUI.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;

namespace MKW.GUI
{
    public interface ITabItemViewModel
    {
        public string Header { get; }
        public ContentControl Content { get; }
        public void OnClose();
    }

    public class WelcomeTabItemViewModel : ViewModelBase, ITabItemViewModel
    {
        public string Header { get; }
        public ContentControl Content { get; }

        public WelcomeTabItemViewModel(MainWindowViewModel mainWindow)
        {
            Header = "Welcome";
            Content = new StartPage(mainWindow);
        }

        public void OnClose()
        {
            throw new NotImplementedException();
        }
    }

    public class DatabaseTabItemViewModel : ViewModelBase, ITabItemViewModel
    {
        private readonly DatabaseViewModel databaseViewModel;
        public string Header { get; }
        public ContentControl Content { get; }

        public DatabaseTabItemViewModel(DatabaseViewModel databaseViewModel)
        {
            this.databaseViewModel = databaseViewModel;

            Header = Path.GetFileNameWithoutExtension(databaseViewModel.Database.Path);
            Content = new DatabasePage(databaseViewModel);
        }


        public void OnClose()
        {
            databaseViewModel.Dispose();
        }
    }

    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private readonly RegistryService registryService;
        private readonly RecentFilesService recentFilesService;

        public ObservableCollection<ITabItemViewModel> TabItems { get; }

        public MainWindowViewModel()
        {
            registryService = new RegistryService(RegistryKeys.RootKeyPath);
            recentFilesService = new RecentFilesService(registryService);
            RecentFiles = new RecentFilesCollectionViewModel(recentFilesService);
            TabItems = new ObservableCollection<ITabItemViewModel>();
            TabItems.Add(new WelcomeTabItemViewModel(this));
        }

        public RecentFilesCollectionViewModel RecentFiles { get; }

        public string Title => "Multi-Key Wallet";
        public string Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!.ToString();

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
                DatabaseTabItemViewModel tabViewModel = new DatabaseTabItemViewModel(new DatabaseViewModel(createDatabaseViewModel.Database /* move */));
                TabItems.Add(tabViewModel);
                SelectedTab = tabViewModel;
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
                DatabaseTabItemViewModel tabViewModel = new DatabaseTabItemViewModel(new DatabaseViewModel(loginWindowViewModel.Database /* move */));
                TabItems.Add(tabViewModel);
                SelectedTab = tabViewModel;
            }
        }

        public void OnCloseTab(ITabItemViewModel selectedTab)
        {
            selectedTab.OnClose();
            TabItems.Remove(selectedTab);
        }

        public void Dispose()
        {
            registryService.Dispose();
            RecentFiles.Dispose();
        }

        private ITabItemViewModel? selectedTab;

        public ITabItemViewModel? SelectedTab
        { 
            get => selectedTab; 
            set => SetProperty(ref selectedTab, value); 
        }
    }
}
