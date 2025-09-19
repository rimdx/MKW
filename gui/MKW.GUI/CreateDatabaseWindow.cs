using MKW.GUI.Images;
using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI
{
    public class CreateDatabaseWindow : WizardWindow
    {
        public CreateDatabaseWindow(CreateDatabaseWizardViewModel viewModel, Window owner)
            : base(viewModel, owner)
        {
            SetIcon(new AddDatabase());
        }
    }
}
