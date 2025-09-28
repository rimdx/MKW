using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MKW.GUI.Database
{
    public partial class DatabaseUnlockedView : UserControl
    {
        private readonly DatabaseUnlockedViewModel model;

        public DatabaseUnlockedView(DatabaseUnlockedViewModel model)
        {
            this.model = model;
            DataContext = model;

            InitializeComponent();

            InfoPage.Content = new PageInfo(model);
            EntriesPage.Content = new PageEntries(model);
            UsersPage.Content = new PageUsers(model);
        }

        private void AddEntryCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                NewEntryWindowViewModel viewModel = model.CreateNewEntryWindowViewModel();
                NewEntryWindow window = new NewEntryWindow(viewModel, Window.GetWindow(this));
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void EditEntry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (model.SelectedEntry != null)
                {
                    using EditEntryWindowViewModel viewModel = model.CreateEditEntryWindowViewModel(model.SelectedEntry.Id);
                    EditEntryWindow window = new EditEntryWindow(viewModel, Window.GetWindow(this));
                    window.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }
    }
}
