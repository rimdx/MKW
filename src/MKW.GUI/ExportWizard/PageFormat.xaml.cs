using MKW.GUI.Wizard;

namespace MKW.GUI.ExportWizard
{
    public partial class PageFormat : WizardPage
    {
        private readonly ExportWizardViewModel viewModel;

        public PageFormat(ExportWizardViewModel viewModel)
            : base("Choose Format")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
