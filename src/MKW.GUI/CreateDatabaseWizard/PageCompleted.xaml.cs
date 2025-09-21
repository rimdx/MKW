using MKW.GUI.Wizard;

namespace MKW.GUI.CreateDatabaseWizard
{
    public partial class PageCompleted : WizardPage
    {
        private CreateDatabaseWizardViewModel viewModel;

        public PageCompleted(CreateDatabaseWizardViewModel viewModel)
            : base("Completed")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
