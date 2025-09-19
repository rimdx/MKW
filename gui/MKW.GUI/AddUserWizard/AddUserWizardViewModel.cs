using MKW.GUI.Wizard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
