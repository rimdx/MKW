using System.Windows;

namespace MKW.GUI
{
    public partial class UserPropertyDialog : Window
    {
        private readonly UserPropertyDialogViewModel model;

        public UserPropertyDialog(UserPropertyDialogViewModel model)
        {
            this.model = model;
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
    }
}
