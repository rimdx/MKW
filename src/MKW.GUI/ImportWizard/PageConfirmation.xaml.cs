using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.ImportWizard
{
    public partial class PageConfirmation : WizardPage
    {
        private readonly ImportWizardViewModel viewModel;

        public PageConfirmation(ImportWizardViewModel viewModel)
            : base("Confirmation")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        public override bool Next()
        {
            try
            {
                viewModel.Confirm();
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
