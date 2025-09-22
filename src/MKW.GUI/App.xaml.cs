using System.Windows;

namespace MKW.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly AppModel model;

        public App()
        {
            model = new AppModel();

            MainWindow = new MainWindow(model);
            MainWindow.Visibility = Visibility.Visible;

            InitializeComponent();
        }
    }

}
