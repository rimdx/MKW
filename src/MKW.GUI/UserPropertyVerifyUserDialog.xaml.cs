using System.Windows;

namespace MKW.GUI
{
    public partial class UserPropertyVerifyUserDialog : DialogWindow
    {
        private readonly UserPropertyDialogViewModel model;

        public UserPropertyVerifyUserDialog(UserPropertyDialogViewModel model, Window owner) : base(owner)
        {
            this.model = model;
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
