using MKW.GUI.Wizard;

namespace MKW.GUI.AddUserWizard
{
    public partial class AddUserWizardConfirmationPage : WizardPage
    {
        private readonly AddUserWizardViewModel viewModel;

        public AddUserWizardConfirmationPage(AddUserWizardViewModel viewModel)
            : base("Confirmation")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
