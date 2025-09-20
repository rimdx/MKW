using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI
{
    public partial class CreateDatabaseMasterPassword : WizardPage
    {
        private readonly CreateDatabaseWizardViewModel viewModel;

        public CreateDatabaseMasterPassword(CreateDatabaseWizardViewModel viewModel)
            : base("Create Master Password")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        public override bool Next()
        {
            try
            {
                string password = ctlPassword.Password;
                string passwordRepeat = ctlPasswordRepeat.Password;

                if (password != passwordRepeat)
                {
                    throw new Exception("Password and repeated password don't match.");
                }

                viewModel.Password = password;

                return true;
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
                return false;
            }
        }
    }
}
