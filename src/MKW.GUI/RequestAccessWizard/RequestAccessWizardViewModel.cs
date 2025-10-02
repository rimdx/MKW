using MKW.Core;
using MKW.Core.Serialization;
using MKW.GUI.Images;
using MKW.GUI.Model;
using MKW.GUI.Services;
using MKW.GUI.Wizard;
using System.IO;

namespace MKW.GUI.RequestAccessWizard
{
    public class RequestAccessWizardViewModel : WizardViewModel
    {

        public RequestAccessWizardViewModel(DatabaseModel database)
            : base(FormatTitle(database), ImageMoniker.NewUser)
        {
            Database = database;
            Password = new PasswordViewModel();

            AddPage(new PageWelcome(this));
            AddPage(new PagePassword(this));
            AddPage(new PageConfirmation(this));
            AddPage(new PageResults(this));
        }

        public DatabaseModel Database { get; private set; }

        private string requestString = "";
        public string RequestString
        {
            get => requestString;
            private set => SetProperty(ref requestString, value);
        }

        public PasswordViewModel Password { get; }

        public void GenerateRequest()
        {
            EnsurePassword();

            UserAccessRequest request = Database.CreateUserAccessRequest(Password.Password);

            KeyFormatter keyFormatter = new KeyFormatter(52);

            RequestString = UserAccessRequestSerializer.Serialize(request);
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
