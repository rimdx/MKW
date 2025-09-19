using MKW.GUI.Wizard;

namespace MKW.GUI.AddUserWizard
{
    public partial class AddUserWizardWelcomePage : WizardPage
    {
        private readonly AddUserWizardViewModel viewModel;

        public AddUserWizardWelcomePage(AddUserWizardViewModel viewModel)
            : base("Welcome")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
