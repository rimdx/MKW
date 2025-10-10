using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.ExportWizard
{
    public class ExportWizardDialog : WizardWindow
    {
        private readonly ExportWizardViewModel viewModel;

        public ExportWizardDialog(ExportWizardViewModel viewModel, Window owner)
            : base(viewModel, owner)
        {
            this.viewModel = viewModel;
        }
    }
}
