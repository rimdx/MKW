using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MKW.GUI.EntryEditor
{
    public partial class PayloadEditorProperties : UserControl
    {
        private EntryEditorViewModelBase ViewModel => (EntryEditorViewModelBase)DataContext;

        public PayloadEditorProperties()
        {
            InitializeComponent();
        }

        private void PropertiesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void AddCustomProperty_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                NewCustomPropertyViewModel dialogModel =
                    ViewModel.NewCustomProperty();
                NewCustomPropertyDialog dialog =
                    new NewCustomPropertyDialog(Window.GetWindow(this), dialogModel);
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void EditCustomProperty_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (ViewModel.SelectedCustomProperty != null)
                {
                    EditCustomPropertyViewModel dialogModel =
                        ViewModel.EditCustomProperty(ViewModel.SelectedCustomProperty);
                    EditCustomPropertyDialog dialog =
                        new EditCustomPropertyDialog(Window.GetWindow(this), dialogModel);
                    dialog.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void DeleteCustomProperty_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }
    }
}
