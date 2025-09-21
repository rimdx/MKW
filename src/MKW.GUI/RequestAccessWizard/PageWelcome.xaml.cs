using MKW.GUI.Wizard;

namespace MKW.GUI.RequestAccessWizard
{
    public partial class PageWelcome : WizardPage
    {
        private readonly RequestAccessWizardViewModel viewModel;

        public PageWelcome(RequestAccessWizardViewModel viewModel)
            : base("Welcome")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
