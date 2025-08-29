using MKW.GUI.Model;
using System.Windows;
using System.Windows.Input;

namespace MKW.GUI
{
    public partial class LoginWindow : Window
    {
        private readonly LoginWindowViewModel model;

        public LoginWindow(LoginWindowViewModel model, Window owner)
        {
            this.model = model;
            this.Owner = owner;
            DataContext = model;
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (model.DoLogin())
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(PasswordInput);
        }
    }
}
