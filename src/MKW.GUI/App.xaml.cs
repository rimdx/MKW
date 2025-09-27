using MKW.GUI.Model;
using MKW.GUI.SingleInstance;
using System.Windows;

namespace MKW.GUI
{
    public partial class App : Application, ISingleInstanceApplication
    {
        private readonly AppModel model;

        public App()
        {
            model = new AppModel();

            InitializeComponent();
        }

        public void InvokeMainInstance(string[] args)
        {
            using (MainWindowViewModel mainWindowViewModel = new MainWindowViewModel(model))
            {
                foreach (string arg in args)
                {
                    if (arg.StartsWith("/") || arg.StartsWith("-"))
                    {
                        // option, skip for now.
                    }
                    else
                    {
                        try
                        {
                            DatabaseTabItemViewModel? tabItem = mainWindowViewModel.GetDatabaseByPath(arg);
                            if (tabItem != null)
                            {
                                mainWindowViewModel.SelectedTab = tabItem;
                            }
                            else
                            {
                                mainWindowViewModel.OpenDatabase(DatabaseModel.Open(arg));
                            }
                        }
                        catch
                        {
                        }
                    }
                }

                MainWindow = new MainWindow(mainWindowViewModel)
                {
                    Visibility = Visibility.Visible
                };

                Run();
            }
        }

        public void InvokeExternalInstance(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
