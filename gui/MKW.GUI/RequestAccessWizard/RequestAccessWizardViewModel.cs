using MKW.Core;
using MKW.Core.Client.AccessRequest;
using MKW.GUI.Images;
using MKW.GUI.Model;
using MKW.GUI.Services;
using MKW.GUI.Wizard;
using System.IO;

namespace MKW.GUI.RequestAccessWizard
{
    public class RequestAccessWizardViewModel : WizardViewModel
    {
        private string userName;
        private string userDisplayName;

        public RequestAccessWizardViewModel(DatabaseModel database)
            : base(FormatTitle(database), ImageMoniker.NewUser)
        {
            Database = database;
            Password = new PasswordViewModel();
            userName = "";
            userDisplayName = "";

            AddPage(new RequestAccessWizardWelcomePage(this));
            AddPage(new RequestAccessWizardUserDetailsPage(this));
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

        public PasswordViewModel Password { get; }

        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }

        public string UserDisplayName
        {
            get => userDisplayName;
            set => SetProperty(ref userDisplayName, value);
        }

        public void GenerateRequest()
        {
            EnsurePassword();

            UserAccessRequest request = Database.Client.CreateUserAccessRequest(Password.Password);

            IAccessRequestSerializer serializer = new JSONAccessRequestSerializer();
            KeyFormatter keyFormatter = new KeyFormatter(52);

            ReadOnlyMemory<byte> data = serializer.Serialize(request);

            RequestString = keyFormatter.GetBase64String(data.Span);
        }

        private static string FormatTitle(DatabaseModel database)
        {
            return $"Request Access - { Path.GetFileName(database.Path) }";
        }

        public void EnsurePassword()
        {
            if (Password.PasswordMismatch)
            {
                throw new Exception("Password and repeated password don't match.");
            }

            if (Password.PasswordLength == 0)
            {
                throw new Exception("Password cannot be empty.");
            }
        }
    }
}
