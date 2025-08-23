using Microsoft.Win32;
using MKW.Core.Client;
using MKW.Core.Storage;
using MKW.Core.Storage.JSON;
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

        public MainWindow()
        {
            model = new MainWindowViewModel();
            DataContext = model;
            InitializeComponent();
        }

        private void OpenDatabase(DatabaseModel database)
        {
            LoginWindow window = new LoginWindow(database);

            window.ShowDialog();

            if (database.User == null)
            {
                database.Dispose();
            }
            else
            {
                DatabaseViewModel dbModel = new DatabaseViewModel(database /* move */);
                Database.Content = new DatabasePage(dbModel /* reference */);
            }
        }

        private void NewDatabase_Click(object sender, RoutedEventArgs e)
        {
            FileDialog dialog = new SaveFileDialog
            {
                FileName = "New Database",
                DefaultExt = ".mkw",
                Filter = "Multi-Key Wallet Database File|*.mkw"
            };

            if (dialog.ShowDialog() == true)
            {
                OpenDatabase(DatabaseModel.Open(dialog.FileName) /* move */);
            }
        }

        private void OpenDatabase_Click(object sender, RoutedEventArgs e)
        {
            FileDialog dialog = new OpenFileDialog
            {
                DefaultExt = ".mkw",
                Filter = "Multi-Key Wallet Database File|*.mkw"
            };

            if (dialog.ShowDialog() == true)
            {
                OpenDatabase(DatabaseModel.Open(dialog.FileName) /* move */);
            }
        }
    }
}
