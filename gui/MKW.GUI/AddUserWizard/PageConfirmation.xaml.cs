using MKW.GUI.Wizard;

namespace MKW.GUI.AddUserWizard
{
    public partial class PageConfirmation : WizardPage
    {
        private readonly AddUserWizardViewModel viewModel;

        public PageConfirmation(AddUserWizardViewModel viewModel)
            : base("Confirmation")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
