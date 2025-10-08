using MKW.GUI.Wizard;

namespace MKW.GUI.ImportWizard
{
    public partial class PageCompleted : WizardPage
    {
        private readonly ImportWizardViewModel viewModel;

        public PageCompleted(ImportWizardViewModel viewModel)
            : base("Completed")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
