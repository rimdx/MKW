using MKW.GUI.Wizard;

namespace MKW.GUI.ImportWizard
{
    public partial class PageFile : WizardPage
    {
        private readonly ImportWizardViewModel viewModel;

        public PageFile(ImportWizardViewModel viewModel)
            : base("Chose File")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
