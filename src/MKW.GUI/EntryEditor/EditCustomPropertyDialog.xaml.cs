using System.Windows;

namespace MKW.GUI.EntryEditor
{
    public partial class EditCustomPropertyDialog : DialogWindow
    {
        private readonly EditCustomPropertyViewModel viewModel;

        public EditCustomPropertyDialog(Window owner, EditCustomPropertyViewModel viewModel)
            : base(owner)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                viewModel.OnOK();
                Close();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(this, ex);
            }
        }
    }
}
