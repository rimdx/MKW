using MKW.GUI.Wizard;

namespace MKW.GUI.RequestAccessWizard
{
    public partial class RequestAccessWizardResultsPage : WizardPage
    {
        private readonly RequestAccessWizardViewModel viewModel;

        public RequestAccessWizardResultsPage(RequestAccessWizardViewModel viewModel)
            : base("Export Access Request")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
