using System.Windows;

namespace MKW.GUI
{
    public partial class UserPropertyDialog : Window
    {
        private readonly UserPropertyDialogViewModel model;

        public UserPropertyDialog(UserPropertyDialogViewModel model, Window window)
        {
            this.model = model;
            Owner = window;
            DataContext = model;
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (model.OnOK())
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(ex);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
        }

        private void VerifyUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserPropertyVerifyUserDialog window = new UserPropertyVerifyUserDialog(model, this);
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(ex);
            }
        }
    }
}
