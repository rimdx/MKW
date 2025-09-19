using MKW.GUI.Wizard;

namespace MKW.GUI
{
    public partial class CreateDatabaseConfirm : WizardPage
    {
        private CreateDatabaseWizardViewModel viewModel;

        public CreateDatabaseConfirm(CreateDatabaseWizardViewModel viewModel)
            : base("Confirm Database Creation")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
