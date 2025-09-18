using MKW.GUI.Wizard;

namespace MKW.GUI
{
    public partial class CreateDatabaseConfirm : WizardPage
    {
        private CreateDatabaseWindowViewModel viewModel;

        public CreateDatabaseConfirm(CreateDatabaseWindowViewModel viewModel)
            : base("Confirm Database Creation")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
