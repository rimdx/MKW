using Microsoft.Win32;
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
            FileDialog dialog = new SaveFileDialog
            {
                FileName = "New Database",
                DefaultExt = ".mkw",
                Filter = "Multi-Key Wallet Database File|*.mkw"
            };

            if (dialog.ShowDialog() == true)
            {
                CreateDatabaseWindowViewModel createDatabaseViewModel =
                    new CreateDatabaseWindowViewModel(dialog.FileName);
                CreateDatabaseWindow createDatabaseWindow =
                    new CreateDatabaseWindow(createDatabaseViewModel);

                createDatabaseWindow.ShowDialog();

                if (createDatabaseViewModel.Database != null)
                {
                    model.Database = new DatabaseViewModel(createDatabaseViewModel.Database /* move */);
                }
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
                DatabaseModel database = DatabaseModel.Open(dialog.FileName);
                LoginWindow window = new LoginWindow(database);

                window.ShowDialog();

                if (database.User == null)
                {
                    database.Dispose();
                }
                else
                {
                    model.Database = new DatabaseViewModel(database /* move */);
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            model.Dispose();
        }

        private void AddEntry_Click(object sender, RoutedEventArgs e)
        {
            NewEntryWindow window = new NewEntryWindow(model.GetDatabase());
            window.ShowDialog();
        }

        private void EditEntry_Click(object sender, RoutedEventArgs e)
        {
            databasePage!.OnEditEntry();
        }
    }
}
