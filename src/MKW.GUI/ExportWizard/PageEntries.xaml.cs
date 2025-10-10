using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.ExportWizard
{
    public partial class PageEntries : WizardPage
    {
        private readonly ExportWizardViewModel viewModel;

        public PageEntries(ExportWizardViewModel viewModel)
            : base("Pick Entries")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
