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
                if (viewModel.PasswordMismatch)
                {
                    throw new Exception("Password and repeated password don't match.");
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
                return false;
            }
        }

        private void ctlPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            viewModel.Password = ctlPassword.Password;
        }

        private void ctlPasswordRepeat_PasswordChanged(object sender, RoutedEventArgs e)
        {
            viewModel.PasswordRepeat = ctlPasswordRepeat.Password;
        }
    }
}
