using MKW.GUI.Wizard;

namespace MKW.GUI.AddUserWizard
{
    public partial class ConfirmationPage : WizardPage
    {
        private readonly AddUserWizardViewModel viewModel;

        public ConfirmationPage(AddUserWizardViewModel viewModel)
            : base("Confirmation")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
