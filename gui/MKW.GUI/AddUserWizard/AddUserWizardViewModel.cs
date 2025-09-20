using MKW.Core;
using MKW.Core.Client.AccessRequest;
using MKW.GUI.Images;
using MKW.GUI.Model;
using MKW.GUI.Wizard;

namespace MKW.GUI.AddUserWizard
{
    public class AddUserWizardViewModel : WizardViewModel
    {
        private readonly DatabaseModel model;

        public AddUserWizardViewModel(DatabaseModel model)
            : base("Add User", ImageMoniker.AddUser)
        {
            userName = "";
            userDisplayName = "";

            this.model = model;

            AddPage(new PageWelcome(this));
            AddPage(new PageImportRequest(this));
            AddPage(new PageUserDetails(this));
            AddPage(new PageConfirmation(this));
            AddPage(new PageCompleted(this));
        }

        private string requestString = "";
        public string RequestString
        {
            get => requestString;
            set => SetProperty(ref requestString, value);
        }

        private UserAccessRequest? request;
        public UserAccessRequest? Request
        {
            get => request;
            set => SetProperty(ref request, value);
        }

        private string userName;
        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }

        private string userDisplayName;
        public string UserDisplayName
        {
            get => userDisplayName;
            set => SetProperty(ref userDisplayName, value);
        }

        public void ParseAccessRequest()
        {
            IAccessRequestSerializer serializer = new JSONAccessRequestSerializer();

            try
            {
                byte[] bytes = Convert.FromBase64String(RequestString);
                Request = serializer.Deserialize(bytes);
            }
            catch (Exception)
            {
                Request = null;
                throw;
            }
        }

        public void DoAddUser()
        {
            if (Request == null)
            {
                throw new Exception("Request is not valid or not specified.");
            }

            model.AddUser(Request);
        }
    }
}
