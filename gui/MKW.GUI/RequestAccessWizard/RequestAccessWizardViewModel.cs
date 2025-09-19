using MKW.GUI.Model;
using MKW.GUI.Wizard;

namespace MKW.GUI.RequestAccessWizard
{
    public class RequestAccessWizardViewModel : WizardViewModel
    {
        public RequestAccessWizardViewModel(DatabaseModel database)
            : base("Request Access")
        {
            Database = database;

            AddPage(new RequestAccessWizardWelcomePage(this));
            AddPage(new RequestAccessWizardPasswordPage(this));
            AddPage(new RequestAccessWizardResultsPage(this));
        }

        public DatabaseModel Database { get; private set; }

        public string RequestString { get; } = "abc";

        public string Password { get; set; } = "";
        public bool IsPasswordMatch { get; set; } = true;
    }
}
