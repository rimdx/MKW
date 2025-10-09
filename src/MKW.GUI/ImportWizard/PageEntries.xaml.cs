using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.ImportWizard
{
    public partial class PageEntries : WizardPage
    {
        private readonly ImportWizardViewModel viewModel;

        public PageEntries(ImportWizardViewModel viewModel)
            : base("Pick Entries")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void EntryListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
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
