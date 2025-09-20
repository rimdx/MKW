using MKW.GUI.Wizard;
using System.Windows;

namespace MKW.GUI.RequestAccessWizard
{
    public partial class RequestAccessWizardPasswordPage : WizardPage
    {
        private readonly RequestAccessWizardViewModel viewModel;

        public RequestAccessWizardPasswordPage(RequestAccessWizardViewModel viewModel)
            : base("Create Password")
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        public override bool Next()
        {
            try
            {
                viewModel.EnsurePassword();
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
