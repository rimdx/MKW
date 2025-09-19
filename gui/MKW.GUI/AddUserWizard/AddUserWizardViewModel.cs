using MKW.GUI.Wizard;

namespace MKW.GUI.AddUserWizard
{
    public class AddUserWizardViewModel : WizardViewModel
    {
        public AddUserWizardViewModel()
            : base("Add User")
        {
            AddPage(new AddUserWizardWelcomePage(this));
            AddPage(new AddUserWizardImportRequestPage(this));
            AddPage(new AddUserWizardConfirmationPage(this));
        }
    }
}
