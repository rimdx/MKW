using System.Windows;
using System.Windows.Controls;

namespace MKW.GUI
{
    public partial class DatabaseUsersPage : UserControl
    {
        private readonly DatabaseViewModel model;

        public DatabaseUsersPage(DatabaseViewModel model)
        {
            this.model = model;
            DataContext = model;
            InitializeComponent();
        }

        private void ListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void NewUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NewUserWindowViewModel viewModel = model.CreateNewUserWindowViewModel();
                NewUserWindow window = new NewUserWindow(viewModel);
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(ex);
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
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this user?",
                                                          "Confirm Deletion",
                                                          MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    model.DeleteUser();
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(ex);
            }
        }
    }
}
