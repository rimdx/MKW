using System.ComponentModel;
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
            model.PropertyChanged += Model_PropertyChanged;
            DataContext = model;
            InitializeComponent();
        }

        private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(model.Database) && model.Database != null)
            {
                Database.Content = new DatabasePage(model.Database);
            }
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
