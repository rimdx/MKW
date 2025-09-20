using MKW.GUI.Wizard;

namespace MKW.GUI.AddUserWizard
{
    public partial class WelcomePage : WizardPage
    {
        private readonly AddUserWizardViewModel viewModel;

        public WelcomePage(AddUserWizardViewModel viewModel)
            : base("Welcome")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
