using MKW.GUI.Wizard;

namespace MKW.GUI.AddUserWizard
{
    public partial class PageWelcome : WizardPage
    {
        private readonly AddUserWizardViewModel viewModel;

        public PageWelcome(AddUserWizardViewModel viewModel)
            : base("Welcome")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
