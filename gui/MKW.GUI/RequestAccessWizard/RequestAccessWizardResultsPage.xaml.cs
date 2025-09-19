using MKW.GUI.Wizard;

namespace MKW.GUI.RequestAccessWizard
{
    public partial class RequestAccessWizardResultsPage : WizardPage
    {
        private readonly RequestAccessWizardViewModel viewModel;

        public RequestAccessWizardResultsPage(RequestAccessWizardViewModel viewModel)
            : base("Database Access Request is Ready!")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
