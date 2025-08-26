using Microsoft.Win32;
using MKW.GUI.Model;
using System.ComponentModel;
using System.Windows;

namespace MKW.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel model;
        private DatabasePage? databasePage;

        public MainWindow()
        {
            model = new MainWindowViewModel();
            model.PropertyChanged += Model_PropertyChanged;
            DataContext = model;

            InitializeComponent();

            StartPage.OpenDatabaseClicked += (sender, e) => model.OnOpenDatabase();
            StartPage.NewDatabaseClicked += (sender, e) => model.OnNewDatabase();
        }

        private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(model.Database) && model.Database != null)
            {
                databasePage = new DatabasePage(model.Database /* reference */);
                Database.Content = databasePage;
            }
        }

        private void NewDatabase_Click(object sender, RoutedEventArgs e)
        {
            model.OnNewDatabase();
        }

        private void OpenDatabase_Click(object sender, RoutedEventArgs e)
        {
            model.OnOpenDatabase();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            model.Dispose();
        }

        private void AddEntry_Click(object sender, RoutedEventArgs e)
        {
            model.Database!.OnAddEntry();
        }

        private void EditEntry_Click(object sender, RoutedEventArgs e)
        {
            model.Database!.OnAddEntry();
        }
    }
}
