using MKW.GUI.AddUserWizard;
using MKW.GUI.RequestAccessWizard;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MKW.GUI.Database
{
    public partial class PageUsers : UserControl
    {
        private readonly DatabaseViewModel model;

        public PageUsers(DatabaseViewModel model)
        {
            this.model = model;
            DataContext = model;
            InitializeComponent();
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void NewUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddUserWizardViewModel viewModel = model.CreateNewUserWindowViewModel();
                AddUserWizardWindow window = new AddUserWizardWindow(viewModel, Window.GetWindow(this));
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void NewAccessRequest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RequestAccessWizardViewModel dialogModel = model.CreateRequestAccessViewModel();
                RequestAccessWizardDialog dialog = new RequestAccessWizardDialog(dialogModel, Window.GetWindow(this));
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void UserPropertiesClick(object sender, RoutedEventArgs e)
        {
            using UserPropertyDialogViewModel viewModel = model.CreateUserPropertiesWindowViewModel();
            UserPropertyDialog dialog = new UserPropertyDialog(viewModel, Window.GetWindow(this));
            dialog.ShowDialog();
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show(Window.GetWindow(this),
                                                          "Are you sure you want to delete this user?",
                                                          "Confirm Deletion",
                                                          MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    model.DeleteUser();
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }
    }
}
