using MKW.GUI.Wizard;

namespace MKW.GUI.RequestAccessWizard
{
    public partial class WelcomePage : WizardPage
    {
        private readonly RequestAccessWizardViewModel viewModel;

        public WelcomePage(RequestAccessWizardViewModel viewModel)
            : base("Welcome")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
