using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.AddUserWizard
{
    public class AddUserWizardWindow : WizardWindow
    {
        private readonly AddUserWizardViewModel viewModel;

        public AddUserWizardWindow(AddUserWizardViewModel viewModel, Window owner)
            : base(viewModel, owner)
        {
            this.viewModel = viewModel;
        }
    }
}
