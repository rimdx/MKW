using MKW.GUI.Model;
using MKW.GUI.Wizard;
using System.IO;

namespace MKW.GUI.RequestAccessWizard
{
    public class RequestAccessWizardViewModel : WizardViewModel
    {
        public RequestAccessWizardViewModel(DatabaseModel database)
            : base(MakeTitle(database))
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

        private static string MakeTitle(DatabaseModel database)
        {
            return $"Request Access - { Path.GetFileName(database.Path) }";
        }
    }
}
