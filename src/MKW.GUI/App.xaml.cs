using MKW.GUI.SingleInstance;
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

        protected override void OnStartup(StartupEventArgs e)
        {
            CommandLineArgs args = CommandLineArgs.Parse(e.Args);

            RunRequest request = new RunRequest(args.Paths);
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
            mainWindowViewModel.HandleRunRequest(request);
            MainWindow.Activate();
        }
    }
}
