using MKW.GUI.Wizard;

namespace MKW.GUI
{
    public partial class CreateDatabaseMasterPassword : WizardPage
    {
        private readonly CreateDatabaseWindowViewModel viewModel;

        public CreateDatabaseMasterPassword(CreateDatabaseWindowViewModel viewModel)
            : base("Create Master Password")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
