using MKW.Core;
using MKW.Core.Client.AccessRequest;
using MKW.Core.Notify;
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

        public override bool Finish()
        {
            if (Request == null)
            {
                throw new Exception("Request is not valid or not specified.");
            }

            model.AddUser(Request);

            return true;
        }
    }
}
