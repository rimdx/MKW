using MKW.GUI.Wizard;

namespace MKW.GUI.RequestAccessWizard
{
    public partial class RequestAccessWizardPasswordPage : WizardPage
    {
        private readonly RequestAccessWizardViewModel viewModel;

        public RequestAccessWizardPasswordPage(RequestAccessWizardViewModel viewModel)
            : base("Create Password")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
