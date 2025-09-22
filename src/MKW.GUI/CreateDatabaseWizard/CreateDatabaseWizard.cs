using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.CreateDatabaseWizard
{
    public class CreateDatabaseWizard : WizardWindow
    {
        public CreateDatabaseWizard(CreateDatabaseWizardViewModel viewModel, Window owner)
            : base(viewModel, owner)
        {
        }
    }
}
