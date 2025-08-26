using System.Windows;
using System.Windows.Controls;

namespace MKW.GUI
{
    public partial class DatabasePage : UserControl
    {
        private readonly DatabaseViewModel model;

        public DatabasePage(DatabaseViewModel model)
        {
            this.model = model;
            DataContext = model;

            InitializeComponent();

            InfoPage.Content = new DatabaseInfoPage(model);
            EntriesPage.Content = new DatabaseEntriesPage(model);
            UsersPage.Content = new DatabaseUsersPage(model);
        }

        private void AddEntry_Click(object sender, RoutedEventArgs e)
        {
            model.OnAddEntry();
        }

        private void EditEntry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using EditEntryWindowViewModel viewModel = model.CreateEditEntryWindowViewModel();
                EditEntryWindow window = new EditEntryWindow(viewModel);
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
