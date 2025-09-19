using MKW.GUI.Wizard;

namespace MKW.GUI.RequestAccessWizard
{
    public partial class RequestAccessWizardWelcomePage : WizardPage
    {
        private readonly RequestAccessWizardViewModel viewModel;

        public RequestAccessWizardWelcomePage(RequestAccessWizardViewModel viewModel)
            : base("Welcome")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
