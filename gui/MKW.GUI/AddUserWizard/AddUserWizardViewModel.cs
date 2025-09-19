using MKW.GUI.Model;
using MKW.GUI.Wizard;

namespace MKW.GUI.AddUserWizard
{
    public class AddUserWizardViewModel : WizardViewModel
    {
        private readonly DatabaseModel model;

        public AddUserWizardViewModel(DatabaseModel model)
            : base("Add User")
        {
            this.model = model;

            AddPage(new AddUserWizardWelcomePage(this));
            AddPage(new AddUserWizardImportRequestPage(this));
            AddPage(new AddUserWizardConfirmationPage(this));
        }

        public string AccessRequestTextBox
        {
            get;
            set;
        }
    }
}
