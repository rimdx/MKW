using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.RequestAccessWizard
{
    public partial class ConfirmationPage : WizardPage
    {
        private readonly RequestAccessWizardViewModel viewModel;

        public ConfirmationPage(RequestAccessWizardViewModel viewModel)
            : base("Confirm Access Request Creation")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        public override bool Next()
        {
            try
            {
                viewModel.GenerateRequest();
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
