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
            MainWindow = new MainWindow(model);
            MainWindow.Visibility = Visibility.Visible;

            Run();
        }

        public void InvokeExternalInstance(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
