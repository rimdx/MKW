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
        private readonly MainWindowModel model;

        public MainWindow()
        {
            model = new MainWindowModel();
            DataContext = model;
            InitializeComponent();
        }

        private void OpenDatabase(IDatabase database, string filename)
        {
            LoginWindow window = new LoginWindow(database, filename);

            window.ShowDialog();

            if (window.User == null)
            {
                window.Client.Dispose();
            }
            else
            {
                DatabaseModel dbModel = model.OpenDatabase(window.Client /* move */,
                                                           window.User /* move */);
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
                JSONDatabaseSession database = JSONDatabaseSession.Open(
                    dialog.FileName, DatabaseOpenMode.OpenOrCreate);
                OpenDatabase(database, dialog.FileName);
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
                JSONDatabaseSession database = JSONDatabaseSession.Open(
                    dialog.FileName, DatabaseOpenMode.OpenOrCreate);
                OpenDatabase(database, dialog.FileName);
            }
        }
    }
}
