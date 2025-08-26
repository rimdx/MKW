using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MKW.GUI
{
    public partial class DatabaseEntriesPage : UserControl
    {
        private readonly DatabaseViewModel model;

        public DatabaseEntriesPage(DatabaseViewModel model)
        {
            this.model = model;
            DataContext = model;
            InitializeComponent();
        }

        private void EntryListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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

        private void CopyPayload_Click(object sender, RoutedEventArgs e)
        {
            model.OnCopyPayload();
        }

        private void AddEntry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NewEntryWindowViewModel viewModel = model.CreateNewEntryWindowViewModel();
                NewEntryWindow window = new NewEntryWindow(viewModel);
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void DuplicateEntry_Click(object sender, RoutedEventArgs e)
        {
            model.OnDuplicateEntry();
        }

        private void DeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            model.OnDeleteEntry();
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
