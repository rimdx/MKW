using MKW.Core;
using MKW.Core.Client.AccessRequest;
using MKW.GUI.Model;
using MKW.GUI.Services;
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
            AddPage(new RequestAccessWizardConfirmationPage(this));
            AddPage(new RequestAccessWizardResultsPage(this));
        }

        public DatabaseModel Database { get; private set; }

        private string requestString = "";
        public string RequestString
        {
            get => requestString;
            private set => SetProperty(ref requestString, value);
        }

        public string Password { get; set; } = "";
        public bool IsPasswordMatch { get; set; } = true;

        public void GenerateRequest()
        {
            if (!IsPasswordMatch)
            {
                throw new Exception("Password and repeated password don't match.");
            }

            UserAccessRequest request = Database.Client.CreateUserAccessRequest(Password);

            IAccessRequestSerializer serializer = new JSONAccessRequestSerializer();
            KeyFormatter keyFormatter = new KeyFormatter(52);

            ReadOnlyMemory<byte> data = serializer.Serialize(request);

            RequestString = keyFormatter.GetBase64String(data.Span);
        }

        private static string MakeTitle(DatabaseModel database)
        {
            return $"Request Access - { Path.GetFileName(database.Path) }";
        }
    }
}
