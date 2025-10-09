using MKW.GUI.Wizard;

namespace MKW.GUI.ImportWizard
{
    public partial class PageFormat : WizardPage
    {
        private readonly ImportWizardViewModel viewModel;

        public PageFormat(ImportWizardViewModel viewModel)
            : base("Choose Format")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
