using MKW.GUI.Wizard;

namespace MKW.GUI.AddUserWizard
{
    public partial class AddUserWizardImportRequestPage : WizardPage
    {
        private readonly AddUserWizardViewModel viewModel;

        public AddUserWizardImportRequestPage(AddUserWizardViewModel viewModel)
            : base("Import User Request")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
