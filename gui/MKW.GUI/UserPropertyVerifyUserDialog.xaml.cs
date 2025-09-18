using System.Windows;

namespace MKW.GUI
{
    public partial class UserPropertyVerifyUserDialog : Window
    {
        private readonly UserPropertyDialogViewModel model;

        public UserPropertyVerifyUserDialog(UserPropertyDialogViewModel model, Window window)
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
                if (model.OnVerify())
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                ErrorReporter.HandleException(Window.GetWindow(this), ex);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
