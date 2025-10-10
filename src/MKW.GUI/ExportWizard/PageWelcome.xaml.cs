using MKW.GUI.Wizard;

namespace MKW.GUI.ExportWizard
{
    public partial class PageWelcome : WizardPage
    {
        private readonly ExportWizardViewModel viewModel;

        public PageWelcome(ExportWizardViewModel viewModel)
            : base("Welcome")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
