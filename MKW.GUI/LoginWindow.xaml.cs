using MKW.GUI.Model;
using System.Windows;
using System.Windows.Input;

namespace MKW.GUI
{
    public partial class LoginWindow : Window
    {
        private readonly LoginWindowViewModel model;

        public LoginWindow(DatabaseModel database)
        {
            model = new LoginWindowViewModel(this, database);
            DataContext = model;
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            model.DoLogin();
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
