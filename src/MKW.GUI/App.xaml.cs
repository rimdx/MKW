using MKW.GUI.Model;
using MKW.GUI.SingleInstance;
using System.IO;
using System.Windows;

namespace MKW.GUI
{
    public partial class App : Application, ISingleInstanceApplication
    {
        private readonly AppModel model;
        private readonly SingleInstanceApplicationManager manager;

        public App()
        {
            model = new AppModel();
            manager = new SingleInstanceApplicationManager(this);

            InitializeComponent();
        }

        private RunRequest ParseCommandLine(string[] args)
        {
            List<string> paths = [];

            foreach (string arg in args)
            {
                if (arg.StartsWith("/") || arg.StartsWith("-"))
                {
                    // option, skip for now.
                }
                else
                {
                    paths.Add(Path.GetFullPath(arg));
                }
            }

            return new RunRequest([.. paths]);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            RunRequest request = ParseCommandLine(e.Args);

            if (manager.Run(request))
            {
                using (MainWindowViewModel mainWindowViewModel = new MainWindowViewModel(model))
                {
                    foreach (string path in request.PathsToOpen)
                    {
                        try
                        {
                            DatabaseTabItemViewModel? tabItem = mainWindowViewModel.GetDatabaseByPath(path);
                            if (tabItem != null)
                            {
                                mainWindowViewModel.SelectedTab = tabItem;
                            }
                            else
                            {
                                mainWindowViewModel.OpenDatabase(DatabaseModel.Open(path));
                            }
                        }
                        catch
                        {
                        }
                    }

                    MainWindow = new MainWindow(mainWindowViewModel, manager)
                    {
                        Visibility = Visibility.Visible
                    };
                }
            }
            else
            {
                Shutdown();
            }
        }

        public void InvokeExternalInstance(RunRequest request)
        {
        }
    }
}
