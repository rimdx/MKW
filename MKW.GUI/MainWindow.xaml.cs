using System.Windows;

namespace MKW.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowModel model;

        public MainWindow()
        {
            model = new MainWindowModel();
            DataContext = model;
            InitializeComponent();
        }

        private void NewDatabase_Click(object sender, RoutedEventArgs e)
        {
            model.NewDatabase();
        }

        private void OpenDatabase_Click(object sender, RoutedEventArgs e)
        {
            model.OpenDatabase();
        }
    }
}
