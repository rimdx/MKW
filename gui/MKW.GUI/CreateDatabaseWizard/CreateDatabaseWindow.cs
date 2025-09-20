using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.CreateDatabaseWizard
{
    public class CreateDatabaseWindow : WizardWindow
    {
        public CreateDatabaseWindow(CreateDatabaseWizardViewModel viewModel, Window owner)
            : base(viewModel, owner)
        {
        }
    }
}
