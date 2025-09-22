using System.Windows;

namespace MKW.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            MainWindow = new MainWindow();
            MainWindow.Visibility = Visibility.Visible;

            InitializeComponent();
        }
    }

}
