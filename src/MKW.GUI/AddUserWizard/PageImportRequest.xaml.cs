using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.AddUserWizard
{
    public partial class PageImportRequest : WizardPage
    {
        private readonly AddUserWizardViewModel viewModel;

        public PageImportRequest(AddUserWizardViewModel viewModel)
            : base("Import User Request")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void ImportFromFile_Click(object sender, RoutedEventArgs e)
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

        public override bool Next()
        {
            try
            {
                viewModel.ParseAccessRequest();
                return true;
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
                return false;
            }
        }
    }
}
