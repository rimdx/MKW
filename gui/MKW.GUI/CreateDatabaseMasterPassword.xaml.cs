using MKW.GUI.Wizard;

namespace MKW.GUI
{
    public partial class CreateDatabaseMasterPassword : WizardPage
    {
        private readonly CreateDatabaseWizardViewModel viewModel;

        public CreateDatabaseMasterPassword(CreateDatabaseWizardViewModel viewModel)
            : base("Create Master Password")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
