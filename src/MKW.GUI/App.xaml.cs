using MKW.GUI.SingleInstance;
using System.IO;
using System.Windows;

namespace MKW.GUI
{
    public partial class App : Application, ISingleInstanceApplication
    {
        private readonly AppModel model;
        private readonly SingleInstanceApplicationManager manager;
        private readonly MainWindowViewModel mainWindowViewModel;

        public App()
        {
            model = new AppModel();
            manager = new SingleInstanceApplicationManager(this);
            mainWindowViewModel = new MainWindowViewModel(model);

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
                mainWindowViewModel.HandleRunRequest(request);

                MainWindow = new MainWindow(mainWindowViewModel, manager)
                {
                    Visibility = Visibility.Visible
                };
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
