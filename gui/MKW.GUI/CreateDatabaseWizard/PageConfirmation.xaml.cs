using MKW.GUI.Wizard;

namespace MKW.GUI.CreateDatabaseWizard
{
    public partial class PageConfirmation : WizardPage
    {
        private CreateDatabaseWizardViewModel viewModel;

        public PageConfirmation(CreateDatabaseWizardViewModel viewModel)
            : base("Confirm Database Creation")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
