using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.CreateDatabaseWizard
{
    public partial class PageMasterPassword : WizardPage
    {
        private readonly CreateDatabaseWizardViewModel viewModel;

        public PageMasterPassword(CreateDatabaseWizardViewModel viewModel)
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
                if (viewModel.Password.PasswordMismatch)
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
            viewModel.Password.Password = ctlPassword.Password;
        }

        private void ctlPasswordRepeat_PasswordChanged(object sender, RoutedEventArgs e)
        {
            viewModel.Password.PasswordRepeat = ctlPasswordRepeat.Password;
        }
    }
}
