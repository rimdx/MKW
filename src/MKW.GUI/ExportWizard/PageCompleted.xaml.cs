using MKW.GUI.Wizard;

namespace MKW.GUI.ExportWizard
{
    public partial class PageCompleted : WizardPage
    {
        private readonly ExportWizardViewModel viewModel;

        public PageCompleted(ExportWizardViewModel viewModel)
            : base("Completed")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
