using MKW.GUI.RequestAccessWizard;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MKW.GUI.Database
{
    public partial class LoginProfileView : UserControl
    {
        public LoginProfileViewModel ViewModel => (LoginProfileViewModel)DataContext;

        public LoginProfileView()
        {
            InitializeComponent();
        }

        private void LoginCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ViewModel.Login();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void RequestAccess_Click(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                RequestAccessWizardViewModel dialogModel = ViewModel.CreateRequestAccessViewModel();
                RequestAccessWizardDialog dialog = new RequestAccessWizardDialog(dialogModel, Window.GetWindow(this));
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }
    }
}
