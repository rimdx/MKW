using MKW.GUI.EntryEditor;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MKW.GUI.Database
{
    public partial class PageEntries : UserControl
    {
        private readonly DatabaseUnlockedViewModel model;

        public PageEntries(DatabaseUnlockedViewModel model)
        {
            this.model = model;
            DataContext = model;
            InitializeComponent();
        }

        private void EntryListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
