using MKW.GUI.EntryEditor;
using MKW.GUI.ImportWizard;
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

        private void EditEntryCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (model.SelectedEntry != null);
        }

        private void EditEntryCommand_Executed(object sender, ExecutedRoutedEventArgs e)
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

        private void DeleteEntryCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (model.SelectedEntry != null);
        }

        private void DeleteEntryCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (model.SelectedEntry != null)
                {
                    MessageBoxResult result = MessageBox.Show(Window.GetWindow(this),
                                                          "Are you sure you want to delete this entry?",
                                                          "Confirm Deletion",
                                                          MessageBoxButton.OKCancel);

                    if (result == MessageBoxResult.OK)
                    {
                        model.DeleteEntry(model.SelectedEntry);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void Import_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ImportWizardViewModel dialogModel = model.CreateImportViewModel();
                ImportWizardDialog dialog = new ImportWizardDialog(dialogModel, Window.GetWindow(this));
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }
    }
}
